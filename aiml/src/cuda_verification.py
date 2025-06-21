# C:\Users\28769\AppData\Local\Programs\Python\Python310\python.exe -m venv .venv310
# .venv310\Scripts\activate

import torch, platform, sys

print("Torch:", torch.__version__)           # 2.2.0+cu121
print("CUDA? ", torch.cuda.is_available())   # True
print("GPU  : ", torch.cuda.get_device_name(0))
print("OS    :", platform.platform())
print("PyVer :", sys.version)
