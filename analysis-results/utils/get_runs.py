import requests
import pandas as pd

_cached_runs = None

def get_runs(force_update=False):
    global _cached_runs
    
    if _cached_runs is not None and not force_update:
        return _cached_runs

    url = 'http://game-runs.glitch.me/runs'
    response = requests.get(url)
    if response.status_code != 200:
        print('Falha na requisição:', response.status_code)
        exit(0)

    runs_data = response.json()['data']
    _cached_runs = pd.DataFrame(runs_data)
    _cached_runs['previous_index'] = None
    get_previous_difficulty_index(_cached_runs)
    _cached_runs['actual_difficulty'] = _cached_runs.apply(
        lambda run: get_actual_difficulty_change(run)
        if run['previous_difficulty'] != 0
        else None, 
        axis=1
    )
    return _cached_runs

def get_actual_difficulty_change(row):
    if row['difficulty'] > row['previous_difficulty']:
        return "MoreDifficult"
    elif row['difficulty'] < row['previous_difficulty']:
        return "Easier"
    else:
        return "Equal"

def get_previous_difficulty_index(runs_df):
    for username in runs_df['username'].unique():
        user_data = runs_df[runs_df['username'] == username]

        for i in range(1, len(user_data)):
            current_row = user_data.iloc[i]
            previous_row = user_data.iloc[i - 1]

            if current_row['previous_difficulty'] == 0.0:
                continue
            
            if previous_row['is_completed'] == 1:
                runs_df.loc[runs_df['id'] == current_row['id'], 'previous_index'] = previous_row['id']
                #if current_row['previous_difficulty'] != previous_row['difficulty']:
                    #print("ERRADO")
                    #print(f'current_row: {current_row}')
                    #print(f"previous_row: {previous_row}")
    
    #num_none = df['previous_index'].isna().sum()
    #print(f'Quantidade de None: {num_none}')
