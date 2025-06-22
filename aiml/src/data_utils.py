import pandas as pd
import numpy as np
from pathlib import Path
import logging

logger = logging.getLogger(__name__)

## COLUMNS = [
##    "Time", "ElapsedTime", "DistanceMeter", "AltitudeMeters",
##    "Speed", "HeartRateBpm", "CadenceRpm", "Power", "Temperature"
##]

def read_all_csv(folder: str) -> pd.DataFrame:
    dfs = []
    
    file_list = list(Path(folder).glob("*.csv"))
    logger.info("📥  Found %d ride files in %s", len(file_list), Path(folder))

    for f in file_list:
        ##df = pd.read_csv(f, names=COLUMNS, header=0)
        df = pd.read_csv(f, header=0)
        df["ride_id"] = f.stem
        dfs.append(df)
        logger.debug("    ↳ %s  (%d rows)", f.name, len(df))

    combo = pd.concat(dfs, ignore_index=True)
    logger.info("🧮  Combined dataframe shape: %s", combo.shape)

    combo["Time"] = pd.to_datetime(
        combo["Time"],
        format="%Y-%m-%dT%H:%M:%SZ",
        utc=True
    )
    combo.sort_values("Time", inplace=True)
    combo.reset_index(drop=True, inplace=True)

    logger.info("util.read_all_csv: return combo")
    return combo     # ← still raw, fix_gaps will handle gaps

def fix_gaps(df: pd.DataFrame) -> pd.DataFrame:
    df = df.copy()
    df = df.set_index("Time")

    full_index = pd.date_range(df.index.min(), df.index.max(), freq="1s")
    df = df.reindex(full_index)
    logger.debug("⏳  After reindex: %d → %d rows (filled gaps)",
                 len(df.index.intersection(full_index)), len(df))

    # 1-second step for the synthetic rows
    df["ElapsedTime"] = df["ElapsedTime"].fillna(1)

    # zero-speed / zero-power during pauses
    df[["Speed", "Power"]] = df[["Speed", "Power"]].fillna(0.0)

    # forward-fill everything else
    ffill_cols = [
        "HeartRateBpm", "CadenceRpm", "AltitudeMeters",
        "DistanceMeter", "Temperature", "ride_id"
    ]
    df[ffill_cols] = df[ffill_cols].ffill()

    df = df.reset_index().rename(columns={"index": "Time"})
    logger.info("util.fix_gaps: return df")
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
    keep_cols   = features + [target, "ride_id"]   # ← retain ride ID
    out = df[keep_cols]
    logger.debug("🛠️  Engineered features DF shape: %s", out.shape)
    logger.info("util.engineer_features: return out")
    return out
