import matplotlib.pyplot as plt
import numpy as np
import datetime
'''
from sklearn.preprocessing import PolynomialFeatures
from sklearn.linear_model import LinearRegression
from sklearn.pipeline import make_pipeline
from sklearn.svm import SVR
'''


data = {}
first = True

log = 15


#enter your log directory
with open(r'D:\User\Desktop\tez\ML\algorithm\tmp\logs\\' + str(log) + 'thLog.txt') as f:
    for line in f:
        line = line[:-1]
        if first:
            first = False

            columns = line.split(",")
            print(columns)
            for col in columns:
                data[col] = []

        else:
            column_data = line.split(",")
            for col, col_value in zip(columns, column_data):
                data[col].append(float(col_value))


total_run_time = data["start_time"][-1] -  data["start_time"][0]
print(str(datetime.timedelta(seconds=total_run_time))) 
print(np.sum(data["update_count"]))

running_average_data = [None]*50
for i in range(100, len(data["avg_reward"])):
    running_average_data.append(np.average(data["avg_reward"][i-100:i]))

running_max_data = [None]*50
for i in range(100, len(data["max_reward"])):
    running_max_data.append(np.average(data["max_reward"][i-100:i]))

running_min_data = [None]*50
for i in range(100, len(data["min_reward"])):
    running_min_data.append(np.average(data["min_reward"][i-100:i]))

running_survive_data = [None]*50
for i in range(100, len(data["episode_time"])):
    running_survive_data.append(np.average(data["episode_time"][i-100:i]))

running_score_data = [None]*50
for i in range(100, len(data["score"])):
    running_score_data.append(np.average(data["score"][i-100:i]))


X = np.arange(len(data["avg_reward"]),dtype=float)
Y = np.array(data["avg_reward"],dtype=float)

idx = np.isfinite(X) & np.isfinite(Y)
X = X[idx].reshape(-1, 1)
Y = Y[idx]
'''
pred = np.arange(0,len(data["avg_reward"])+2000).reshape(-1,1)

svr_poly = SVR(kernel='poly', C=100, gamma='auto', degree=3, epsilon=.1, coef0=1)

Y_NEW = svr_poly.fit(X, Y).predict(pred)
'''
"""
degree=5
polyreg=make_pipeline(PolynomialFeatures(degree),LinearRegression())
polyreg.fit(X,Y)

Y_NEW= polyreg.predict(pred)
"""

fig1, ax1 = plt.subplots()
ax1.plot(data["avg_reward"])
ax1.plot(running_average_data)
ax1.set_title("Durschnittlische Belohnung")
ax1.set_xlabel('Episoden')
ax1.set_ylabel("BelohnungsWerte")
fig1.show()

fig2, ax2 = plt.subplots()
ax2.plot(data["min_reward"])
ax2.plot(running_min_data)
ax2.set_title("Min Belohnung")
ax2.set_xlabel('Episoden')
ax2.set_ylabel("BelohnungsWerte")
fig2.show()

fig3, ax3 = plt.subplots()
ax3.plot(data["max_reward"])
ax3.plot(running_max_data)
ax3.set_title("Max Belohnung")
ax3.set_xlabel('Episoden')
ax3.set_ylabel("BelohnungsWerte")
fig3.show()

fig4, ax4 = plt.subplots()
ax4.plot(data["episode_time"])
ax4.plot(running_survive_data)
ax4.set_title("Episode Dauer" )
ax4.set_xlabel('Episoden')
ax4.set_ylabel("Episode Dauer in Sekunden")
fig4.show()


fig5, ax5 = plt.subplots()
ax5.plot(data["score"])
ax5.plot(running_score_data)
ax5.set_title("Episoden Punkte" )
ax5.set_xlabel('Episoden')
ax5.set_ylabel("Episoden Punkte")
fig5.show()

fig6, ax6 = plt.subplots()
ax6.plot(running_score_data)
ax6.set_title("Gleitende Mittelwert von Episoden Punkte" )
ax6.set_xlabel('Episoden')
ax6.set_ylabel("Episoden Punkte")
fig6.show()

fig7, ax7 = plt.subplots()
ax7.plot(running_average_data)
ax7.set_title("Gleitende Mittelwert von durchschnittlichen Belohnung pro Episode" )
ax7.set_xlabel('Episoden')
ax7.set_ylabel("Episoden Punkte")
fig7.show()

'''
fig8, ax8 = plt.subplots()
ax8.plot(running_average_data)
ax8.plot(Y_NEW)
ax8.set_title("Running average average" )
ax8.set_xlabel('Episoden')
ax8.set_ylabel("Episoden Punkte")
fig8.show()
'''
plt.show()