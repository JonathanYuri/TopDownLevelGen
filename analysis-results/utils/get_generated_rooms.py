import requests
import pandas as pd
import numpy as np
from .get_fitness_vars import get_fitness_vars_by_id

_cached_generated_rooms = None

def create_percent_column(generated_rooms):
    vars_columns = ['NumEnemiesGroupId', 'AverageEnemiesPerGroupId',
                'AverageEnemyDoorDistanceId', 'AverageBetweenEnemiesDistancesId',
                'AverageObstaclesNextToEnemiesId', 'AverageEnemiesWithCoverId']

    var_max_value = 100
    percent_values = []

    for _, row in generated_rooms.iterrows():
        fitness_vars_id = int(row['fitness_vars_id'])
        fitness_vars_row = get_fitness_vars_by_id(fitness_vars_id)

        qnt_variables = 0
        for var_column in vars_columns:
            var_properties_id = fitness_vars_row[var_column].values[0]
            if not np.isnan(var_properties_id):
                qnt_variables += 1

        percent = float(row['best_value'] / (qnt_variables * var_max_value)) if qnt_variables > 0 else 0
        percent_values.append(percent)

    generated_rooms['percent'] = percent_values


def get_generated_rooms(force_update=False):
    global _cached_generated_rooms

    if _cached_generated_rooms is not None and not force_update:
        return _cached_generated_rooms

    response = requests.get(f'http://game-runs.glitch.me/generated-rooms')
    if response.status_code != 200:
        print('Falha na requisição:', response.status_code)
        exit(0)

    generated_rooms_data = response.json()['data']
    _cached_generated_rooms = pd.DataFrame(generated_rooms_data)
    create_percent_column(_cached_generated_rooms)
    return _cached_generated_rooms