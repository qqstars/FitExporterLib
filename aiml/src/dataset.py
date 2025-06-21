import torch
from torch.utils.data import Dataset
import numpy as np
from sklearn.preprocessing import StandardScaler
import joblib   # for saving the scaler

class CyclingWindowDataset(Dataset):
    def __init__(self, npz_path: str, window: int = 5, scaler_path: str = None, mode="train"):
        """
        npz contains two numpy arrays: X_raw, y_raw (full time-series)
        We build sliding windows inside __getitem__.
        """
        data = np.load(npz_path)
        X_raw, y_raw = data["X"], data["y"]
        ride_ids = data["ride_id"]

        if mode == "train":
            self.scaler = StandardScaler().fit(X_raw)
            joblib.dump(self.scaler, scaler_path or "scaler.gz")
        else:
            self.scaler = joblib.load(scaler_path or "scaler.gz")

        X_raw = self.scaler.transform(X_raw)

        # build sequences
        self.window = window
        seqs, targets = [], []
        for i in range(window - 1, len(X_raw)):
            if ride_ids[i] != ride_ids[i - window + 1]:
                continue  # skip if window crosses sessions
            seqs.append(X_raw[i-window+1:i+1])
            targets.append(y_raw[i])
        self.X = torch.tensor(np.array(seqs), dtype=torch.float32)
        self.y = torch.tensor(np.array(targets), dtype=torch.float32).unsqueeze(-1)

    def __len__(self):
        return len(self.X)

    def __getitem__(self, idx):
        return self.X[idx], self.y[idx]
