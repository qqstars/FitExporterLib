import torch, numpy as np, pandas as pd
from data_utils import fix_gaps, engineer_features
from dataset import CyclingWindowDataset
from model import LSTMPower
import joblib, sys

CSV_PATH = sys.argv[1]   # absolute path to new ride CSV

# Preprocess the single file
df = pd.read_csv(CSV_PATH, names=[
    "Time","ElapsedTime","DistanceMeter","AltitudeMeters","Speed",
    "HeartRateBpm","CadenceRpm","Power","Temperature"
])
df["Time"] = pd.to_datetime(df["Time"])
df = fix_gaps(df)
df = engineer_features(df)

X_raw = df.drop(columns=["Power"]).values
scaler = joblib.load("data/processed/scaler.gz")
X_raw = scaler.transform(X_raw)

WINDOW = 5
seqs = []
for i in range(WINDOW - 1, len(X_raw)):
    seqs.append(X_raw[i - WINDOW + 1: i + 1])
X = torch.tensor(np.array(seqs), dtype=torch.float32)

model = LSTMPower(n_feats=X.shape[-1])
model.load_state_dict(torch.load("checkpoints/best_lstm.pt", map_location="cpu"))
model.eval()

with torch.no_grad():
    preds = model(X).squeeze().numpy()

df = df.iloc[WINDOW - 1:].copy()
df["PredictedPower"] = preds
print(df[["Time", "Speed", "CadenceRpm", "PredictedPower"]].head(20))
