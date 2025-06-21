import pandas as pd
import numpy as np
from pathlib import Path

COLUMNS = [
    "Time", "ElapsedTime", "DistanceMeter", "AltitudeMeters",
    "Speed", "HeartRateBpm", "CadenceRpm", "Power", "Temperature"
]

def read_all_csv(folder: str) -> pd.DataFrame:
    dfs = []
    for f in Path(folder).glob("*.csv"):
        df = pd.read_csv(f, names=COLUMNS)
        df["ride_id"] = f.stem          # e.g., 2023-05-06
        dfs.append(df)
    combo = pd.concat(dfs, ignore_index=True)
    combo["Time"] = pd.to_datetime(combo["Time"])
    combo.sort_values("Time", inplace=True)
    combo.reset_index(drop=True, inplace=True)
    return combo

def fix_gaps(df: pd.DataFrame) -> pd.DataFrame:
    """
    Fill the missing seconds introduced when the computer paused logging.
    We re-index to a 1 Hz timeline and forward-fill last known values
    except Power & Speed, which go to 0 during pauses.
    """
    df = df.copy()
    df.set_index("Time", inplace=True)
    full_index = pd.date_range(df.index.min(), df.index.max(), freq="1S")
    df = df.reindex(full_index)
    df["ElapsedTime"].fillna(1, inplace=True)             # 1 s step
    df[["Speed", "Power"]] = df[["Speed", "Power"]].fillna(0.0)
    df[["HeartRateBpm", "CadenceRpm", "AltitudeMeters",
        "DistanceMeter", "Temperature"]] = df[[
            "HeartRateBpm", "CadenceRpm", "AltitudeMeters",
            "DistanceMeter", "Temperature"]].ffill()
    df.reset_index(inplace=True)
    df.rename(columns={"index": "Time"}, inplace=True)
    return df

def engineer_features(df: pd.DataFrame) -> pd.DataFrame:
    df = df.copy()

    # Convert the unit of sppeed from km/h to m/s
    # Speed (m/s)  ⬆︎  (Speed in km/h) * (1/3.6)
    KPH_TO_MS = 1/3.6
    df["speed_ms"] = df["Speed"] * KPH_TO_MS

    # Acceleration (m/s²)
    df["accel_ms2"] = df["speed_ms"].diff().fillna(0)

    # Distance delta (m)
    df["delta_dist"] = df["DistanceMeter"].diff().fillna(0)

    # Slope (%)  ⬆︎  (Δalt / Δdist) * 100 – guard against zero distance
    df["slope_pct"] = np.where(
        df["delta_dist"] > 0,
        (df["AltitudeMeters"].diff().fillna(0) / df["delta_dist"]) * 100,
        0.0
    ).clip(-30, 30)  # tame outliers

    # Rolling means smooth sensors a bit
    for col in ["speed_ms", "accel_ms2", "slope_pct", "HeartRateBpm", "CadenceRpm"]:
        df[f"{col}_smooth"] = df[col].rolling(window=3, min_periods=1).mean()

    # Choose model-input columns
    features = [
        "speed_ms_smooth",
        "accel_ms2_smooth",
        "slope_pct_smooth",
        "HeartRateBpm_smooth",
        "CadenceRpm_smooth",
        "Temperature"
    ]
    target = "Power"
    return df[features + [target]]
