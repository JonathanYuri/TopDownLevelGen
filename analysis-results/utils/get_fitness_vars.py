import requests
import pandas as pd

_cached_fitness_vars = None

def get_fitness_vars(force_update=False):
    global _cached_fitness_vars

    if _cached_fitness_vars is not None and not force_update:
        return _cached_fitness_vars

    response = requests.get(f'http://game-runs.glitch.me/fitness-vars')
    if response.status_code != 200:
        print('Falha na requisição:', response.status_code)
        exit(0)

    fitness_vars_data = response.json()['data']
    _cached_fitness_vars = pd.DataFrame(fitness_vars_data)
    return _cached_fitness_vars

def get_fitness_vars_by_id(id, force_update=False):
    fitness_vars = get_fitness_vars(force_update)
    return fitness_vars.loc[fitness_vars['id'] == id]