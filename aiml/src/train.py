import torch, os
from torch.utils.data import DataLoader, random_split
from .dataset import CyclingWindowDataset
from .model import LSTMPower
from pathlib import Path
import pandas as pd
import numpy as np

import logging
logging.basicConfig(level=logging.INFO,
                    format="%(asctime)s %(levelname)-8s %(message)s",
                    datefmt="%H:%M:%S")
logger = logging.getLogger(__name__)

# Constants
WINDOW = 15
BATCH  = 128
EPOCHS = 20
LR     = 1e-3

# 1) Prepare a single combined NPZ (X_raw, y_raw) for simplicity
def make_npz(out="data/processed/all.npz"):
    from src.data_utils import read_all_csv, fix_gaps, engineer_features

    out_path = Path(out)
    if out_path.exists():
        logger.info("📦  %s already exists (%.1f MB) – skipping build",
                    out_path, out_path.stat().st_size / 1_048_576)
        return

    logger.info("🔄  Building %s …", out_path)
    raw_df = read_all_csv("data/raw")          # concat with ride_id column

    dfs = []
    for ride, ride_df in raw_df.groupby("ride_id", sort=False):
        ride_df = fix_gaps(ride_df)            # gap-fill within *each* ride
        ride_df = engineer_features(ride_df)
        dfs.append(ride_df)

    df = pd.concat(dfs, ignore_index=True)
    logger.info("📊  After gap-fill: %d rows", len(df))

    X_raw    = df.drop(columns=["Power", "ride_id"]).values.astype("float32")
    y_raw    = df["Power"].values.astype("float32")
    ride_ids = df["ride_id"].astype("U32").values          # 1-D unicode array

    out_path.parent.mkdir(parents=True, exist_ok=True)
    np.savez_compressed(out_path, X=X_raw, y=y_raw, ride_ids=ride_ids)
    logger.info("✅  Saved %s (%.1f MB)", out_path, out_path.stat().st_size / 1_048_576)


logger.info("🚀  starting make_npz()")
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

# Log dataset info
logger.info("📊  Total sequences: %d  |  Train: %d  Val: %d",
            len(dataset), train_len, val_len)
logger.info("🖥️   Device: %s", device)

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

    logger.info(f"Epoch {epoch:02d}  |  Val MAE = {mae:6.2f} W")

    if mae < best_mae:
        best_mae = mae
        Path("checkpoints").mkdir(parents=True, exist_ok=True)
        torch.save(model.state_dict(), "checkpoints/best_lstm.pt")

logger.info("✅  Training complete! Best MAE: %.2f W", best_mae)
