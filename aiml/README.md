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

# Environment
- Python version required: 3.10 https://www.python.org/downloads/release/python-31018/   https://www.python.org/downloads/windows/
  - If the computer has multiple Python versions, can switch:
    - Check installed Python: `where.exe python`
    - Modify the `/aiml/src/cuda_verification.py`, enabled the commented lines:
    ```
    # <YOUR_PYTHON_PATH>\Python310\python.exe -m venv .venv310
    # .venv310\Scripts\activate
    ```
    - OR, we can switch the default Python version by:
      - Create or modify the file from: `%programdata%\py.ini`
      - Add content:
      ```
      [defaults]
      python=3.10
      ```
- Install CUDA 12.9: https://developer.nvidia.com/cuda-downloads
  - Verify the version: `nvcc --version`

# Installation of Environment:
  - Create Virtual Environment from `aiml` folder, if the default version is 3.10.
  ```
  py -m venv .venv
  .venv\Scripts\activate
  pip install -r requirements.txt
  ```
  - Or use particular version if default version is any other versions:
  ```
  py -3.10 -m venv --without-pip .venv310
  curl https://bootstrap.pypa.io/get-pip.py -o get-pip.py
  .venv310\Scripts\python get-pip.py

  .venv310\Scripts\activate
  pip install -r requirements.txt
  ```

# Initialize Data for execution
  - Create folder: `data/processed` and `data/raw` under `aiml` folder.
  - Copy all converted CSV file from FitExporter into the `data/raw` folder.

# Execution:
  - Run command from virtual Environment:
  ```
  python -m src.train
  ```

