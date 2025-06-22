## The aiml folder contains the machine learning model and its training data

The model is built using NumPy for raw data processing 
The CSV data files are processed and saved into the data/processed/ folder

This ML model uses PyTorch for 

# Project Structure
```
cycling_power_model/
│
├── data/
│   ├── raw/                ← put ALL your CSV rides here
│   └── processed/          ← numpy “*.npz” files after preprocessing
│
├── src/
│   ├── data_utils.py       ← load / clean / feature-engineer
│   ├── dataset.py          ← PyTorch Dataset & DataLoader wrappers
│   ├── model.py            ← LSTM (or GRU / MLP) definition
│   ├── train.py            ← training loop, metrics, checkpoints
│   └── infer.py            ← quick script to predict on a new CSV
│
├── notebooks/              ← (optional) Jupyter EDA, plots
├── requirements.txt
└── README.md
```

# Installation:
```
python -m venv .venv
.venv\Scripts\activate
pip install -r requirements.txt
```
