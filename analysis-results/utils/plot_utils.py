def plot_for_each_level(df, plot_function):
    for level in range(0, 3):
        df_filtered = df[df['level'] == level]
        plot_function(df_filtered, level)