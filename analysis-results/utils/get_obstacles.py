import requests
import pandas as pd

_cached_obstacles = None

def get_obstacles(force_update=False):
    global _cached_obstacles
    
    if _cached_obstacles is not None and not force_update:
        return _cached_obstacles

    response = requests.get(f'http://game-runs.glitch.me/obstacles')
    if response.status_code != 200:
        print('Falha na requisição:', response.status_code)
        exit(0)

    obstacles_data = response.json()['data']
    _cached_obstacles = pd.DataFrame(obstacles_data)
    return _cached_obstacles

def get_obstacles_by_run_indexes(indexes, force_update=False):
    obstacles = get_obstacles(force_update)
    return obstacles.loc[obstacles['run_id'].isin(indexes)]

def get_obstacles_by_run_id(run_id, force_update=False):
    obstacles = get_obstacles(force_update)
    return obstacles[obstacles['run_id'] == run_id]
