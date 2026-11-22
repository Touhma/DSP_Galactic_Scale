#!/usr/bin/env python3
"""
Build Assets/constellations.json (1,000 stars across all 88 IAU constellations + exotic remnants).

Sources:
- HYG v4.0 Database (astronexus/HYG-Database)
- Stellarium modern skyculture (88 constellation stick figures + IAU common names)
- Stellarium chinese skyculture (traditional Chinese star names)
- Curated real Black Holes, Neutron Stars, White Dwarfs at true J2000 coordinates
"""

import argparse
import csv
import gzip
import json
import math
import os
import re
import urllib.request
from pathlib import Path

PC_TO_LY = 3.26156378

URLS = {
    "hyg": "https://raw.githubusercontent.com/astronexus/HYG-Database/main/hyg/CURRENT/hygdata_v40.csv.gz",
    "modern": "https://raw.githubusercontent.com/Stellarium/stellarium/master/skycultures/modern/index.json",
    "chinese": "https://raw.githubusercontent.com/Stellarium/stellarium/master/skycultures/chinese/star_names.zh_CN.fab",
}

CON_ZH = {
    "And": "仙女座", "Ant": "唧筒座", "Aps": "天燕座", "Aqr": "宝瓶座",
    "Aql": "天鹰座", "Ara": "天坛座", "Ari": "白羊座", "Aur": "御夫座",
    "Boo": "牧夫座", "Cae": "雕具座", "Cam": "鹿豹座", "Cnc": "巨蟹座",
    "CVn": "猎犬座", "CMa": "大犬座", "CMi": "小犬座", "Cap": "摩羯座",
    "Car": "船底座", "Cas": "仙后座", "Cen": "半人马座", "Cep": "仙王座",
    "Cet": "鲸鱼座", "Cha": "蝘蜓座", "Cir": "圆规座", "Col": "天鸽座",
    "Com": "后发座", "CrA": "南冕座", "CrB": "北冕座", "Crv": "乌鸦座",
    "Crt": "巨爵座", "Cru": "南十字座", "Cyg": "天鹅座", "Del": "海豚座",
    "Dor": "剑鱼座", "Dra": "天龙座", "Equ": "小马座", "Eri": "波江座",
    "For": "天炉座", "Gem": "双子座", "Gru": "天鹤座", "Her": "武仙座",
    "Hor": "时钟座", "Hya": "长蛇座", "Hyi": "水蛇座", "Ind": "印第安座",
    "Lac": "蝎虎座", "Leo": "狮子座", "LMi": "小狮座", "Lep": "天兔座",
    "Lib": "天秤座", "Lup": "豺狼座", "Lyn": "天猫座", "Lyr": "天琴座",
    "Men": "山案座", "Mic": "显微镜座", "Mon": "麒麟座", "Mus": "苍蝇座",
    "Nor": "矩尺座", "Oct": "南极座", "Oph": "蛇夫座", "Ori": "猎户座",
    "Pav": "孔雀座", "Peg": "飞马座", "Per": "英仙座", "Phe": "凤凰座",
    "Pic": "绘架座", "Psc": "双鱼座", "PsA": "南鱼座", "Pup": "船尾座",
    "Pyx": "罗盘座", "Ret": "网罟座", "Sge": "天箭座", "Sgr": "人马座",
    "Sco": "天蝎座", "Scl": "玉夫座", "Sct": "盾牌座", "Ser": "巨蛇座",
    "Sex": "六分仪座", "Tau": "金牛座", "Tel": "望远镜座", "Tri": "三角座",
    "TrA": "南三角座", "Tuc": "杜鹃座", "UMa": "大熊座", "UMi": "小熊座",
    "Vel": "船帆座", "Vir": "室女座", "Vol": "飞鱼座", "Vul": "狐狸座",
}

BAYER_GREEK = {
    "alp": "α", "bet": "β", "gam": "γ", "del": "δ", "eps": "ε", "zet": "ζ",
    "eta": "η", "the": "θ", "iot": "ι", "kap": "κ", "lam": "λ", "mu": "μ",
    "nu": "ν", "xi": "ξ", "omi": "ο", "pi": "π", "rho": "ρ", "sig": "σ",
    "tau": "τ", "ups": "υ", "phi": "φ", "chi": "χ", "psi": "ψ", "ome": "ω",
}

SUPERSCRIPTS = {"0": "⁰", "1": "¹", "2": "²", "3": "³", "4": "⁴", "5": "⁵", "6": "⁶", "7": "⁷", "8": "⁸", "9": "⁹"}

COLLOQUIAL_ZH = {
    32349: "天狼星",
    91262: "织女星",
    97649: "牛郎星",
    11767: "北极星",
    30438: "老人星",
    69673: "大角星",
    70890: "比邻星",
    87937: "巴纳德星",
}

PRIORITY_NEIGHBOR_HIPS = {70890, 87937, 114046}

# 26 prominent naked-eye constellations (12 Zodiac + 14 iconic sky landmarks)
# prioritized for smaller star budgets (e.g., 450-640 stars on mid-range PCs)
PROMINENT_CONSTELLATIONS = {
    # 12 Zodiac
    "Ari", "Tau", "Gem", "Cnc", "Leo", "Vir",
    "Lib", "Sco", "Sgr", "Cap", "Aqr", "Psc",
    # 14 Iconic naked-eye landmarks (Orion, Big/Little Dipper, Cassiopeia, Summer Triangle, Southern Cross, etc.)
    "Ori", "UMa", "UMi", "Cas", "Cyg", "Lyr", "Aql",
    "Cru", "CMa", "Cen", "Per", "Aur", "Boo", "Peg",
}

CURATED_REMNANTS = [
    # 8 Black Holes
    {"nameEn": "Cygnus X-1", "nameZh": "天鹅座X-1 (天鹅座 HDE 226868)", "con": "Cyg", "starType": "BlackHole", "spectr": "X", "ra_h": 19.9727, "dec_d": 35.2017, "dist_ly": 7240.0, "mag": 8.95, "lum": 0.006, "temp": 0.0, "mass": 21.2, "radius": 4.0},
    {"nameEn": "Gaia BH1", "nameZh": "盖亚BH1 (蛇夫座 BH1)", "con": "Oph", "starType": "BlackHole", "spectr": "X", "ra_h": 17.4781, "dec_d": -0.5811, "dist_ly": 1560.0, "mag": 13.8, "lum": 0.0055, "temp": 0.0, "mass": 9.6, "radius": 3.8},
    {"nameEn": "Gaia BH3", "nameZh": "盖亚BH3 (天鹰座 BH3)", "con": "Aql", "starType": "BlackHole", "spectr": "X", "ra_h": 19.6552, "dec_d": 14.9317, "dist_ly": 1926.0, "mag": 11.2, "lum": 0.0062, "temp": 0.0, "mass": 32.7, "radius": 4.2},
    {"nameEn": "A0620-00", "nameZh": "麒麟座V616 (麒麟座 A0620-00)", "con": "Mon", "starType": "BlackHole", "spectr": "X", "ra_h": 6.3790, "dec_d": -0.3456, "dist_ly": 3460.0, "mag": 18.3, "lum": 0.0058, "temp": 0.0, "mass": 6.6, "radius": 3.7},
    {"nameEn": "Sagittarius A*", "nameZh": "人马座A* (人马座银心黑洞)", "con": "Sgr", "starType": "BlackHole", "spectr": "X", "ra_h": 17.7611, "dec_d": -29.0078, "dist_ly": 26670.0, "mag": 20.0, "lum": 0.0072, "temp": 0.0, "mass": 65.0, "radius": 4.5},
    {"nameEn": "V404 Cygni", "nameZh": "天鹅座V404 (天鹅座 GS 2023+338)", "con": "Cyg", "starType": "BlackHole", "spectr": "X", "ra_h": 20.4011, "dec_d": 33.8672, "dist_ly": 7800.0, "mag": 18.4, "lum": 0.006, "temp": 0.0, "mass": 9.0, "radius": 3.9},
    {"nameEn": "GRS 1915+105", "nameZh": "天鹰座V1487 (天鹰座 GRS 1915+105)", "con": "Aql", "starType": "BlackHole", "spectr": "X", "ra_h": 19.2532, "dec_d": 10.9458, "dist_ly": 28000.0, "mag": 19.0, "lum": 0.0068, "temp": 0.0, "mass": 12.4, "radius": 4.1},
    {"nameEn": "GX 339-4", "nameZh": "天坛座V821 (天坛座 GX 339-4)", "con": "Ara", "starType": "BlackHole", "spectr": "X", "ra_h": 17.0471, "dec_d": -48.7897, "dist_ly": 26000.0, "mag": 17.0, "lum": 0.0061, "temp": 0.0, "mass": 5.9, "radius": 3.8},
    # 8 Neutron Stars
    {"nameEn": "Crab Pulsar", "nameZh": "天关客星脉冲星 (金牛座 PSR B0531+21)", "con": "Tau", "starType": "NeutronStar", "spectr": "X", "ra_h": 5.5755, "dec_d": 22.0145, "dist_ly": 6500.0, "mag": 16.5, "lum": 0.35, "temp": 20000000.0, "mass": 1.4, "radius": 0.45},
    {"nameEn": "Vela Pulsar", "nameZh": "船帆座脉冲星 (船帆座 PSR B0833-45)", "con": "Vel", "starType": "NeutronStar", "spectr": "X", "ra_h": 8.5891, "dec_d": -45.1764, "dist_ly": 936.0, "mag": 23.6, "lum": 0.32, "temp": 18000000.0, "mass": 1.4, "radius": 0.42},
    {"nameEn": "Geminga", "nameZh": "杰敏卡脉冲星 (双子座 PSR J0633+1746)", "con": "Gem", "starType": "NeutronStar", "spectr": "X", "ra_h": 6.5651, "dec_d": 17.7703, "dist_ly": 815.0, "mag": 25.5, "lum": 0.29, "temp": 15000000.0, "mass": 1.4, "radius": 0.4},
    {"nameEn": "Lich", "nameZh": "巫妖脉冲星 (室女座 PSR B1257+12)", "con": "Vir", "starType": "NeutronStar", "spectr": "X", "ra_h": 13.0008, "dec_d": 12.6822, "dist_ly": 2300.0, "mag": 24.0, "lum": 0.31, "temp": 22000000.0, "mass": 1.4, "radius": 0.44},
    {"nameEn": "PSR J0437-4715", "nameZh": "绘架座脉冲星 (绘架座 PSR J0437-4715)", "con": "Pic", "starType": "NeutronStar", "spectr": "X", "ra_h": 4.6211, "dec_d": -47.2525, "dist_ly": 510.0, "mag": 20.0, "lum": 0.28, "temp": 14000000.0, "mass": 1.44, "radius": 0.41},
    {"nameEn": "RX J1856.5-3754", "nameZh": "南冕座孤立中子星 (南冕座 RX J1856)", "con": "CrA", "starType": "NeutronStar", "spectr": "X", "ra_h": 18.9433, "dec_d": -37.9097, "dist_ly": 400.0, "mag": 25.7, "lum": 0.3, "temp": 16000000.0, "mass": 1.4, "radius": 0.4},
    {"nameEn": "Cassiopeia A", "nameZh": "仙后座A中子星 (仙后座 Cas A)", "con": "Cas", "starType": "NeutronStar", "spectr": "X", "ra_h": 23.3911, "dec_d": 58.8117, "dist_ly": 11000.0, "mag": 22.0, "lum": 0.38, "temp": 25000000.0, "mass": 1.5, "radius": 0.46},
    {"nameEn": "Calvera", "nameZh": "卡尔维拉中子星 (小熊座 1RXS J1412)", "con": "UMi", "starType": "NeutronStar", "spectr": "X", "ra_h": 14.2156, "dec_d": 79.3678, "dist_ly": 6500.0, "mag": 26.0, "lum": 0.33, "temp": 19000000.0, "mass": 1.4, "radius": 0.43},
    # 10 White Dwarfs
    {"nameEn": "Van Maanen's Star", "nameZh": "范马南星 (双鱼座 HIP 3829)", "con": "Psc", "starType": "WhiteDwarf", "spectr": "X", "ra_h": 0.8192, "dec_d": 5.3886, "dist_ly": 14.07, "mag": 12.37, "lum": 0.05, "temp": 135000.0, "mass": 0.67, "radius": 0.25},
    {"nameEn": "40 Eridani B", "nameZh": "九州殊口增十一B (波江座40B)", "con": "Eri", "starType": "WhiteDwarf", "spectr": "X", "ra_h": 4.2546, "dec_d": -7.6581, "dist_ly": 16.34, "mag": 9.52, "lum": 0.06, "temp": 145000.0, "mass": 0.57, "radius": 0.28},
    {"nameEn": "Stein 2051 B", "nameZh": "施泰因2051B (鹿豹座 WD 0426+58)", "con": "Cam", "starType": "WhiteDwarf", "spectr": "X", "ra_h": 4.5204, "dec_d": 58.9772, "dist_ly": 17.99, "mag": 12.43, "lum": 0.045, "temp": 128000.0, "mass": 0.68, "radius": 0.26},
    {"nameEn": "LP 145-141", "nameZh": "苍蝇座白矮星 (苍蝇座 HIP 57367)", "con": "Mus", "starType": "WhiteDwarf", "spectr": "X", "ra_h": 11.7575, "dec_d": -64.8414, "dist_ly": 15.12, "mag": 11.5, "lum": 0.052, "temp": 132000.0, "mass": 0.75, "radius": 0.27},
    {"nameEn": "Lucy", "nameZh": "露西钻石星 (半人马座V886)", "con": "Cen", "starType": "WhiteDwarf", "spectr": "X", "ra_h": 12.6472, "dec_d": -49.8, "dist_ly": 48.0, "mag": 13.7, "lum": 0.048, "temp": 140000.0, "mass": 1.1, "radius": 0.24},
    {"nameEn": "IK Pegasi B", "nameZh": "飞马座IK B (飞马座白矮星)", "con": "Peg", "starType": "WhiteDwarf", "spectr": "X", "ra_h": 21.4411, "dec_d": 19.3767, "dist_ly": 154.0, "mag": 13.0, "lum": 0.075, "temp": 165000.0, "mass": 1.15, "radius": 0.23},
    {"nameEn": "AR Scorpii", "nameZh": "天蝎座AR (天蝎座白矮脉冲星)", "con": "Sco", "starType": "WhiteDwarf", "spectr": "X", "ra_h": 16.3632, "dec_d": -22.8861, "dist_ly": 384.0, "mag": 13.9, "lum": 0.068, "temp": 155000.0, "mass": 0.81, "radius": 0.29},
    {"nameEn": "WD 1145+017", "nameZh": "室女座白矮星 (室女座 WD 1145+017)", "con": "Vir", "starType": "WhiteDwarf", "spectr": "X", "ra_h": 11.8097, "dec_d": 1.4831, "dist_ly": 570.0, "mag": 17.2, "lum": 0.062, "temp": 150000.0, "mass": 0.6, "radius": 0.3},
    {"nameEn": "ZZ Ceti", "nameZh": "鲸鱼座ZZ (鲸鱼座脉动白矮星)", "con": "Cet", "starType": "WhiteDwarf", "spectr": "X", "ra_h": 1.6036, "dec_d": -11.3431, "dist_ly": 103.0, "mag": 14.1, "lum": 0.058, "temp": 142000.0, "mass": 0.62, "radius": 0.29},
    {"nameEn": "HZ 43", "nameZh": "后发座HZ 43 (后发座白矮星)", "con": "Com", "starType": "WhiteDwarf", "spectr": "X", "ra_h": 13.2727, "dec_d": 29.0989, "dist_ly": 127.0, "mag": 12.9, "lum": 0.088, "temp": 174000.0, "mass": 0.68, "radius": 0.31},
]


def ensure_file(cache_dir: Path, filename: str, url: str) -> Path:
    path = cache_dir / filename
    if not path.exists() or path.stat().st_size == 0:
        req = urllib.request.Request(url, headers={"User-Agent": "Mozilla/5.0"})
        with urllib.request.urlopen(req, timeout=30) as resp:
            path.write_bytes(resp.read())
    return path


def radec_to_xyz_pc(ra_h: float, dec_d: float, dist_pc: float):
    ra = ra_h * (math.pi / 12.0)
    dec = math.radians(dec_d)
    cdec = math.cos(dec)
    return (
        dist_pc * cdec * math.cos(ra),
        dist_pc * cdec * math.sin(ra),
        dist_pc * math.sin(dec),
    )


def format_bayer(bayer_raw: str) -> str:
    if not bayer_raw:
        return ""
    b = bayer_raw.strip()
    m = re.match(r"^([A-Za-z]+)(?:[-]?(\d+))?$", b)
    if not m:
        return ""
    greek = BAYER_GREEK.get(m.group(1).lower())
    if not greek:
        return ""
    num = m.group(2)
    if num:
        sup = "".join(SUPERSCRIPTS.get(c, c) for c in num)
        return f"{greek}{sup}"
    return greek


def estimate_temp_mass_radius(spect_letter: str, star_type: str, lum: float, ci: float):
    if star_type == "WhiteDwarf":
        return 145000.0, 0.7, 0.27
    if star_type == "NeutronStar":
        return 20000000.0, 1.5, 0.43
    if star_type == "BlackHole":
        return 0.0, 20.0, 4.0

    if not math.isnan(ci):
        # Ballesteros (2012) approximation
        bv = max(-0.35, min(2.0, ci))
        temp = 4600.0 * (1.0 / (0.92 * bv + 1.7) + 1.0 / (0.92 * bv + 0.62))
    else:
        temp_map = {"O": 36000.0, "B": 18000.0, "A": 8800.0, "F": 6700.0, "G": 5700.0, "K": 4600.0, "M": 3500.0}
        temp = temp_map.get(spect_letter, 5700.0)

    lum = max(0.001, lum)
    if star_type == "GiantStar":
        mass_map = {"O": 28.0, "B": 14.0, "A": 6.0, "F": 4.0, "G": 3.0, "K": 2.4, "M": 2.0}
        mass = mass_map.get(spect_letter, 3.0)
        raw_r = math.sqrt(lum) * ((5778.0 / max(2200.0, temp)) ** 2)
        radius = max(10.0, min(36.0, raw_r * 0.4 + 12.0))
    else:
        mass_map = {"O": 24.0, "B": 7.0, "A": 2.1, "F": 1.4, "G": 1.0, "K": 0.78, "M": 0.5}
        mass = mass_map.get(spect_letter, 1.0)
        raw_r = math.sqrt(lum) * ((5778.0 / max(2500.0, temp)) ** 2)
        radius = max(0.45, min(5.5, raw_r))
    return round(temp, 1), round(mass, 3), round(radius, 3)


def parse_spectral_and_type(spect_raw: str, ci: float, absmag: float):
    s = (spect_raw or "").strip()
    if s.startswith("D"):
        return "WhiteDwarf", "X"
    if s.startswith(("W", "WN", "WC", "WO")):
        return "GiantStar", "O"
    if s.startswith(("C", "S", "N", "R")):
        return "GiantStar", "M"

    letter = None
    for ch in s.upper():
        if ch in ("O", "B", "A", "F", "G", "K", "M"):
            letter = ch
            break
    if letter is None:
        if math.isnan(ci):
            letter = "G"
        elif ci < -0.25:
            letter = "O"
        elif ci < 0.0:
            letter = "B"
        elif ci < 0.30:
            letter = "A"
        elif ci < 0.58:
            letter = "F"
        elif ci < 0.81:
            letter = "G"
        elif ci < 1.40:
            letter = "K"
        else:
            letter = "M"

    # Parse MK luminosity class (I, Ia, Ib, II, III vs IV, V, VI) allowing punctuation/subtypes like F7:Ib-IIv
    lum_match = re.search(r"[OBAFGKM]\d?(?:\.\d)?[:/+\-]*(Ia0?|Ib|Iab|III|II|IV|VI|V|I)(?:\b|[^A-Za-z]|$)", s)
    if lum_match:
        lc = lum_match.group(1)
        star_type = "GiantStar" if lc in ("Ia0", "Ia", "Ib", "Iab", "I", "II", "III") else "MainSeqStar"
    else:
        # Fallback using absolute magnitude if MK class omitted in HYG
        star_type = "GiantStar" if (not math.isnan(absmag) and absmag < 0.5 and letter in ("G", "K", "M")) else "MainSeqStar"

    return star_type, letter


def parse_float(val: str, default=float("nan")) -> float:
    try:
        return float(val) if val not in ("", None) else default
    except ValueError:
        return default


def parse_int(val: str):
    try:
        return int(float(val)) if val not in ("", None) else None
    except ValueError:
        return None


REPO_ROOT = Path(__file__).resolve().parent.parent


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--cache-dir", default=str(REPO_ROOT / ".cache" / "constellations"))
    parser.add_argument("--output", default=str(REPO_ROOT / "Assets" / "constellations.json"))
    parser.add_argument("--count", type=int, default=1000)
    args = parser.parse_args()

    cache_dir = Path(args.cache_dir)
    cache_dir.mkdir(parents=True, exist_ok=True)
    hyg_path = ensure_file(cache_dir, "hygdata_v40.csv.gz", URLS["hyg"])
    modern_path = ensure_file(cache_dir, "modern_index.json", URLS["modern"])
    zh_path = ensure_file(cache_dir, "star_names.zh_CN.fab", URLS["chinese"])

    with open(modern_path, "r", encoding="utf-8") as f:
        modern = json.load(f)

    # Extract 88 constellations and stick-figure vertex HIP IDs
    constellation_vertices = set()
    constellation_hips_by_con = {}
    for c in modern["constellations"]:
        cid = c["id"].split()[-1]
        hips = set()
        for poly in c.get("lines", []):
            for hip in poly:
                try:
                    h = int(hip)
                except (ValueError, TypeError):
                    continue
                hips.add(h)
                constellation_vertices.add(h)
        constellation_hips_by_con[cid] = hips

    iau_common_en = {}
    for key, entries in modern.get("common_names", {}).items():
        if key.startswith("HIP ") and entries:
            try:
                hip = int(key.split()[1])
                name = entries[0].get("english", "").strip()
                if name:
                    iau_common_en[hip] = name
            except (ValueError, IndexError):
                pass

    zh_trad_by_hip = {}
    with open(zh_path, "r", encoding="utf-8") as f:
        for line in f:
            m = re.match(r'^(\d+)\|_?\("([^"]+)"\)', line.strip())
            if m:
                hip = int(m.group(1))
                name = m.group(2).strip()
                # Keep primary traditional Chinese asterism name (first entry per HIP in file)
                if hip not in zh_trad_by_hip and name:
                    zh_trad_by_hip[hip] = name

    for hip, col_name in COLLOQUIAL_ZH.items():
        zh_trad_by_hip[hip] = col_name

    # Load HYG v4.0 stars
    sol_row = None
    hyg_rows = []
    by_hip = {}
    with gzip.open(hyg_path, "rt", encoding="utf-8", errors="replace") as f:
        reader = csv.DictReader(f)
        for row in reader:
            hyg_id = parse_int(row.get("id"))
            if hyg_id == 0:
                sol_row = row
                continue
            hip = parse_int(row.get("hip"))
            con = (row.get("con") or "").strip()
            mag = parse_float(row.get("mag"), 99.0)
            ra_h = parse_float(row.get("ra"))
            dec_d = parse_float(row.get("dec"))
            dist_pc = parse_float(row.get("dist"))
            is_priority_neighbor = hip in PRIORITY_NEIGHBOR_HIPS if hip is not None else False
            if math.isnan(ra_h) or math.isnan(dec_d) or (mag > 11.0 and not is_priority_neighbor):
                continue

            is_vertex = hip in constellation_vertices if hip is not None else False
            # Handle HYG sentinel dist=100000 for stars without Hipparcos parallax
            if math.isnan(dist_pc) or dist_pc <= 0.0 or dist_pc >= 99000.0:
                if not is_vertex:
                    continue
                dist_pc = 300.0
                x, y, z = radec_to_xyz_pc(ra_h, dec_d, dist_pc)
                vx, vy, vz = 0.0, 0.0, 0.0
            else:
                x = parse_float(row.get("x"), 0.0)
                y = parse_float(row.get("y"), 0.0)
                z = parse_float(row.get("z"), 0.0)
                vx = parse_float(row.get("vx"), 0.0)
                vy = parse_float(row.get("vy"), 0.0)
                vz = parse_float(row.get("vz"), 0.0)

            rec = {
                "hyg_id": hyg_id,
                "hip": hip,
                "hr": parse_int(row.get("hr")),
                "con": con,
                "proper": (row.get("proper") or "").strip(),
                "bayer": (row.get("bayer") or "").strip(),
                "flam": parse_int(row.get("flam")),
                "ra_h": ra_h,
                "dec_d": dec_d,
                "dist_pc": dist_pc,
                "mag": mag,
                "absmag": parse_float(row.get("absmag")),
                "spect": (row.get("spect") or "").strip(),
                "ci": parse_float(row.get("ci")),
                "lum": parse_float(row.get("lum"), 1.0),
                "x": x, "y": y, "z": z,
                "vx": vx, "vy": vy, "vz": vz,
                "is_vertex": is_vertex,
            }
            if con in CON_ZH:
                hyg_rows.append(rec)
                if hip is not None and (hip not in by_hip or rec["mag"] < by_hip[hip]["mag"]):
                    by_hip[hip] = rec

    def build_entry(rec):
        hip = rec["hip"]
        con = rec["con"]
        con_zh = CON_ZH[con]
        greek = format_bayer(rec["bayer"])
        flam = rec["flam"]
        hr = rec["hr"]

        proper_en = rec["proper"] or (iau_common_en.get(hip, "") if hip is not None else "")
        if proper_en:
            name_en = proper_en
        elif greek:
            name_en = f"{greek} {con}"
        elif flam is not None:
            name_en = f"{flam} {con}"
        elif hr is not None:
            name_en = f"HR {hr} ({con})"
        elif hip is not None:
            name_en = f"HIP {hip} ({con})"
        else:
            name_en = f"HYG {rec['hyg_id']} ({con})"

        if hip == 70890:
            zh_con_desig = "半人马座αC"
        elif hip == 87937:
            zh_con_desig = "蛇夫座V2500"
        elif greek:
            zh_con_desig = f"{con_zh}{greek}"
        elif flam is not None:
            zh_con_desig = f"{con_zh}{flam}"
        elif hr is not None:
            zh_con_desig = f"{con_zh} HR {hr}"
        elif hip is not None:
            zh_con_desig = f"{con_zh} HIP {hip}"
        else:
            zh_con_desig = f"{con_zh} HYG {rec['hyg_id']}"

        zh_trad = zh_trad_by_hip.get(hip, "") if hip is not None else ""
        if zh_trad and zh_trad != zh_con_desig:
            name_zh = f"{zh_trad} ({zh_con_desig})"
        else:
            name_zh = zh_con_desig

        star_type, spectr = parse_spectral_and_type(rec["spect"], rec["ci"], rec["absmag"])
        lum = 1.0 if math.isnan(rec["lum"]) or rec["lum"] <= 0.0 else rec["lum"]
        temp, mass, radius = estimate_temp_mass_radius(spectr, star_type, lum, rec["ci"])

        # Compute exact unit vector for angular separation deduplication
        norm = math.sqrt(rec["x"] ** 2 + rec["y"] ** 2 + rec["z"] ** 2)
        ux, uy, uz = (rec["x"] / norm, rec["y"] / norm, rec["z"] / norm) if norm > 0 else (1.0, 0.0, 0.0)

        return {
            "nameEn": name_en,
            "nameZh": name_zh,
            "con": con,
            "starType": star_type,
            "spectr": spectr,
            "x": round(rec["x"], 6),
            "y": round(rec["y"], 6),
            "z": round(rec["z"], 6),
            "vx": round(rec["vx"], 9),
            "vy": round(rec["vy"], 9),
            "vz": round(rec["vz"], 9),
            "mag": round(rec["mag"], 3),
            "lum": round(lum, 5),
            "temp": temp,
            "mass": mass,
            "radius": radius,
            "_u": (ux, uy, uz),
            "_hip": hip,
        }

    # Build Sol entry (index 0)
    catalog = [{
        "nameEn": "Sol",
        "nameZh": "太阳",
        "con": "Sol",
        "starType": "MainSeqStar",
        "spectr": "G",
        "x": 0.0, "y": 0.0, "z": 0.0,
        "vx": 0.0, "vy": 0.0, "vz": 0.0,
        "mag": -26.7,
        "lum": 1.0,
        "temp": 5778.0,
        "mass": 1.0,
        "radius": 1.0,
    }]

    selected_optical_units = []
    used_hyg_ids = set()
    used_names_en = {"Sol"}
    cos_min_sep = math.cos(math.radians(0.20))

    def try_add_optical(rec, force_min_sep=True) -> bool:
        if rec["hyg_id"] in used_hyg_ids:
            return False
        entry = build_entry(rec)
        if entry["nameEn"] in used_names_en:
            return False
        u = entry["_u"]
        if force_min_sep:
            for ou in selected_optical_units:
                if u[0] * ou[0] + u[1] * ou[1] + u[2] * ou[2] > cos_min_sep:
                    return False
        used_hyg_ids.add(rec["hyg_id"])
        used_names_en.add(entry["nameEn"])
        selected_optical_units.append(u)
        del entry["_u"]
        del entry["_hip"]
        catalog.append(entry)
        return True

    # Sort all valid optical HYG rows by apparent magnitude
    hyg_by_mag = sorted(hyg_rows, key=lambda r: r["mag"])

    # Index 1: Always keep Proxima Centauri (比邻星) immediately after Sol so every galaxy size
    # includes Sol's closest real stellar neighbor before Rigil Kentaurus / α Cen
    if 70890 in by_hip:
        try_add_optical(by_hip[70890], force_min_sep=False)

    # Anchor tier (indices 2..89):
    # 1 brightest star from each of the 88 constellations
    by_con = {c: [] for c in CON_ZH}
    for rec in hyg_by_mag:
        by_con[rec["con"]].append(rec)

    anchors = []
    for con in CON_ZH:
        candidates = by_con[con]
        if not candidates:
            raise RuntimeError(f"No stars found for constellation {con}")
        anchors.append(candidates[0])
    anchors.sort(key=lambda r: r["mag"])
    for rec in anchors:
        try_add_optical(rec, force_min_sep=False)

    # First 3 BH, 3 NS, 3 WD in priority band so even 100-star galaxies have all remnant types
    def format_remnant(item):
        dist_pc = item["dist_ly"] / PC_TO_LY
        x, y, z = radec_to_xyz_pc(item["ra_h"], item["dec_d"], dist_pc)
        return {
            "nameEn": item["nameEn"],
            "nameZh": item["nameZh"],
            "con": item["con"],
            "starType": item["starType"],
            "spectr": item["spectr"],
            "x": round(x, 6),
            "y": round(y, 6),
            "z": round(z, 6),
            "vx": 0.0, "vy": 0.0, "vz": 0.0,
            "mag": item["mag"],
            "lum": item["lum"],
            "temp": item["temp"],
            "mass": item["mass"],
            "radius": item["radius"],
        }

    bhs = [format_remnant(r) for r in CURATED_REMNANTS if r["starType"] == "BlackHole"]
    nss = [format_remnant(r) for r in CURATED_REMNANTS if r["starType"] == "NeutronStar"]
    wds = [format_remnant(r) for r in CURATED_REMNANTS if r["starType"] == "WhiteDwarf"]

    catalog.extend(bhs[:3] + nss[:3] + wds[:3])

    # Add brightest O-type MainSeqStar in priority band so all 7 main-seq types exist early too
    o_main = [r for r in hyg_by_mag if parse_spectral_and_type(r["spect"], r["ci"], r["absmag"]) == ("MainSeqStar", "O")]
    for rec in o_main[:1]:
        try_add_optical(rec)

    # Include Barnard's Star (巴纳德星) and brightest M-type main-seq dwarfs if budget permits
    if 87937 in by_hip and len(catalog) < args.count:
        try_add_optical(by_hip[87937])
    m_main = [r for r in hyg_by_mag if parse_spectral_and_type(r["spect"], r["ci"], r["absmag"]) == ("MainSeqStar", "M")]
    for rec in sorted(m_main, key=lambda r: r["mag"])[:3]:
        if len(catalog) < args.count:
            try_add_optical(rec)

    # Tier 2a: Prominent constellations' bright naked-eye stick-figure vertices (mag <= 4.5)
    # Ensures recognizable outlines of all 12 Zodiac + 14 iconic constellations by ~350 stars.
    vertex_recs = sorted([r for r in hyg_by_mag if r["is_vertex"]], key=lambda r: r["mag"])
    for rec in vertex_recs:
        if len(catalog) >= args.count:
            break
        if rec["con"] in PROMINENT_CONSTELLATIONS and rec["mag"] <= 4.5:
            try_add_optical(rec)

    # Tier 2b: Remaining bright naked-eye stars (mag <= 4.0) in prominent constellations
    # (e.g., Pleiades/Alcyone, Lesath, Hatysa, Alniyat, Albaldah, Almaaz, Haedus, Subra).
    for rec in hyg_by_mag:
        if len(catalog) >= args.count:
            break
        if rec["mag"] > 4.0:
            break
        if rec["con"] in PROMINENT_CONSTELLATIONS:
            try_add_optical(rec)

    # Tier 3a: Remaining curated remnants (all 26 BH/NS/WD) + additional O MainSeq stars
    # + all remaining bright naked-eye stars (mag <= 4.0) across the rest of the sky.
    # By ~455 stars, 100% of the mag <= 4.0 sky + all 26 remnants + all 26 prominent outlines are present.
    for rem in bhs[3:] + nss[3:] + wds[3:]:
        if len(catalog) < args.count:
            catalog.append(rem)

    for rec in o_main[1:4]:
        if len(catalog) < args.count:
            try_add_optical(rec)

    for rec in hyg_by_mag:
        if len(catalog) >= args.count:
            break
        if rec["mag"] > 4.0:
            break
        try_add_optical(rec)

    # Tier 3b: Bright stick-figure vertices (mag <= 4.5) of the remaining 62 constellations
    for rec in vertex_recs:
        if len(catalog) >= args.count:
            break
        if rec["mag"] <= 4.5:
            try_add_optical(rec)

    # Tier 4a: Remaining prominent-constellation naked-eye stars (4.0 < mag <= 4.5)
    for rec in hyg_by_mag:
        if len(catalog) >= args.count:
            break
        if rec["mag"] > 4.5:
            break
        if rec["con"] in PROMINENT_CONSTELLATIONS:
            try_add_optical(rec)

    # Tier 4b: Remaining faint stick-figure vertices (mag > 4.5) across obscure constellations
    for rec in vertex_recs:
        if len(catalog) >= args.count:
            break
        try_add_optical(rec)

    # Tier 4c: Fill remainder to exact target count with brightest stars overall
    for rec in hyg_by_mag:
        if len(catalog) >= args.count:
            break
        try_add_optical(rec)

    catalog = catalog[:args.count]
    out_path = Path(args.output)
    out_path.parent.mkdir(parents=True, exist_ok=True)
    with open(out_path, "w", encoding="utf-8") as f:
        json.dump(catalog, f, ensure_ascii=False, indent=2)

    print(f"Wrote {len(catalog)} stars to {out_path}")


if __name__ == "__main__":
    main()
