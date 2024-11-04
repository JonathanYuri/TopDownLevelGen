import requests
import pandas as pd

_cached_variable_properties = None

def get_variable_properties(force_update=False):
    global _cached_variable_properties
    
    if _cached_variable_properties is not None and not force_update:
        return _cached_variable_properties

    response = requests.get(f'http://game-runs.glitch.me/variable-properties')
    if response.status_code != 200:
        print('Falha na requisição:', response.status_code)
        exit(0)

    variable_properties_data = response.json()['data']
    _cached_variable_properties = pd.DataFrame(variable_properties_data)
    return _cached_variable_properties

def get_variable_properties_by_id(id, force_update=False):
    variable_properties = get_variable_properties(force_update)
    return variable_properties.loc[variable_properties['id'] == id]