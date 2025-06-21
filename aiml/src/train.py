import torch, os
from torch.utils.data import DataLoader, random_split
from dataset import CyclingWindowDataset
from model import LSTMPower
from pathlib import Path
import numpy as np

# Constants
WINDOW = 5
BATCH  = 128
EPOCHS = 20
LR     = 1e-3

# 1) Prepare a single combined NPZ (X_raw, y_raw) for simplicity
def make_npz(out="data/processed/all.npz"):
    from data_utils import read_all_csv, fix_gaps, engineer_features
    df = read_all_csv("data/raw")
    df = fix_gaps(df)
    df = engineer_features(df)
    X_raw = df.drop(columns=["Power"]).values
    y_raw = df["Power"].values
    Path(out).parent.mkdir(parents=True, exist_ok=True)
    np.savez_compressed(out, X=X_raw, y=y_raw)

make_npz()

dataset = CyclingWindowDataset(
    npz_path="data/processed/all.npz",
    window=WINDOW,
    scaler_path="data/processed/scaler.gz",
    mode="train"
)

train_len = int(len(dataset) * 0.8)
val_len   = len(dataset) - train_len
train_ds, val_ds = random_split(dataset, [train_len, val_len])

train_loader = DataLoader(train_ds, BATCH, shuffle=True)
val_loader   = DataLoader(val_ds, BATCH)

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
model = LSTMPower(n_feats=dataset.X.shape[-1]).to(device)
optimizer = torch.optim.Adam(model.parameters(), lr=LR)
criterion = torch.nn.MSELoss()

best_mae = 1e9
for epoch in range(1, EPOCHS + 1):
    model.train()
    for X, y in train_loader:
        X, y = X.to(device), y.to(device)
        optimizer.zero_grad()
        loss = criterion(model(X), y)
        loss.backward()
        optimizer.step()

    # validation
    model.eval()
    with torch.no_grad():
        mae = torch.tensor(0.0, device=device)
        n   = 0
        for X, y in val_loader:
            X, y = X.to(device), y.to(device)
            preds = model(X)
            mae += torch.sum(torch.abs(preds - y))
            n   += y.size(0)
        mae = (mae / n).item()

    print(f"Epoch {epoch:02d}  |  Val MAE = {mae:6.2f} W")

    if mae < best_mae:
        best_mae = mae
        torch.save(model.state_dict(), "checkpoints/best_lstm.pt")
