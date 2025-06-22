import torch.nn as nn

class LSTMPower(nn.Module):
    def __init__(self, n_feats: int, hidden: int = 64, layers: int = 2):
        super().__init__()
        self.lstm = nn.LSTM(
            input_size=n_feats,
            hidden_size=hidden,
            num_layers=layers,
            batch_first=True,
            dropout=0.2
        )
        self.fc = nn.Sequential(
            nn.Linear(hidden, 32),
            nn.ReLU(),
            nn.Linear(32, 1)
        )

    def forward(self, x):
        # x: [batch, seq_len, n_feats]
        out, _ = self.lstm(x)
        last = out[:, -1, :]           # take the output at final timestep
        return self.fc(last)
