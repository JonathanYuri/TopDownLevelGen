import requests
import pandas as pd

_cached_enemies = None

def get_enemies(force_update=False):
    global _cached_enemies
    
    if _cached_enemies is not None and not force_update:
        return _cached_enemies

    response = requests.get(f'http://game-runs.glitch.me/enemies')
    if response.status_code != 200:
        print('Falha na requisição:', response.status_code)
        exit(0)

    enemies_data = response.json()['data']
    _cached_enemies = pd.DataFrame(enemies_data)
    return _cached_enemies

def get_enemies_by_run_indexes(indexes, force_update=False):
    enemies = get_enemies(force_update)
    return enemies.loc[enemies['run_id'].isin(indexes)]

def get_enemies_by_run_id(run_id, force_update=False):
    enemies = get_enemies(force_update)
    return enemies[enemies['run_id'] == run_id]
