import torch
from torch.utils.data import Dataset
import numpy as np
from sklearn.preprocessing import StandardScaler
import joblib

class CyclingWindowDataset(Dataset):
    def __init__(self, npz_path: str, window: int = 5,
                 scaler_path: str | None = None,
                 mode: str = "train"):
        data      = np.load(npz_path, allow_pickle=True)
        X_raw     = data["X"]                  # (T, F)
        y_raw     = data["y"]                  # (T,)
        ride_ids  = data["ride_ids"]           # (T,)

        # fit or load scaler
        scaler_path = scaler_path or "scaler.gz"
        if mode == "train":
            self.scaler = StandardScaler().fit(X_raw)
            joblib.dump(self.scaler, scaler_path)
        else:
            self.scaler = joblib.load(scaler_path)

        self.X       = self.scaler.transform(X_raw).astype(np.float32)
        self.y       = y_raw.astype(np.float32)
        self.window  = window
        self.ride_ids = ride_ids

        # indices whose *entire* window stays inside one ride
        mask = self.ride_ids[window-1:] == self.ride_ids[:-window+1]
        self.valid_idx = np.where(mask)[0] + (window - 1)

    def __len__(self):
        return len(self.valid_idx)

    def __getitem__(self, idx):
        j = self.valid_idx[idx]
        seq = torch.from_numpy(self.X[j - self.window + 1 : j + 1])
        target = torch.tensor(self.y[j]).unsqueeze(-1)
        return seq, target