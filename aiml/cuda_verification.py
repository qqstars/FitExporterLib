# C:\Users\28769\AppData\Local\Programs\Python\Python310\python.exe -m venv .venv310
# .venv310\Scripts\activate
# python cuda_verification.py

import torch, platform, sys, pandas, numpy
print("NumPy  :", numpy.__version__)   # 1.26.4
print("pandas :", pandas.__version__)  # 2.2.2
print("Torch:", torch.__version__)           # 2.3.0+cu121
print("CUDA? ", torch.cuda.is_available())   # True
print("GPU  : ", torch.cuda.get_device_name(0))
print("OS    :", platform.platform())
print("PyVer :", sys.version)
