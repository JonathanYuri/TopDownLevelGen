enemy_weights = {
    "Enemy1": 1,
    "Enemy2": 3,
    "Enemy3": 2,
}

obstacle_weights = {
    "Obstacle1": 1,
    "Obstacle2": 3,
}

level_cost = {
    0: 9,
    1: 12,
    2: 15,
}

enemy_names = list(enemy_weights.keys())
obstacle_names = list(obstacle_weights.keys())

name_mapping = {
    "Enemy1": "Cobra",
    "Enemy2": "Esqueleto",
    "Enemy3": "Slime",
    "Obstacle1": "Pedra",
    "Obstacle2": "Espinhos",
    "Feedback": "Feedback"
}

enemy_name_map = {
    "Enemy1": "Cobra",
    "Enemy2": "Esqueleto",
    "Enemy3": "Slime"
}

obstacle_name_map = {
    "Obstacle1": "Pedra",
    "Obstacle2": "Espinhos"
}

feedback_order = ['Easier', 'Equal', 'MoreDifficult']

feedback_mapping = {
    'MoreDifficult': 3,
    'Equal': 2,
    'Easier': 1
}

vars_columns = ['NumEnemiesGroupId', 'AverageEnemiesPerGroupId',
                'AverageEnemyDoorDistanceId', 'AverageBetweenEnemiesDistancesId',
                'AverageObstaclesNextToEnemiesId', 'AverageEnemiesWithCoverId']