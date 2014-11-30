import csv
import datetime
import pytz
import numpy
import matplotlib.pyplot as plt

routes = list(csv.reader(open("routes.csv","rt")))

timezone = pytz.timezone("US/Pacific")
startDate = timezone.localize(datetime.datetime(2014,10,27,0,0,0,0))

# extract an array [time_since_start_in_hours, duration_in_minutes]
samples = []
for route in routes:
    if route[0] == 'HomeToMicrosoft':
        date = datetime.datetime.strptime(route[1],'%m/%d/%Y %I:%M:%S %p')
        date = date.replace(tzinfo=pytz.utc)
        date = timezone.normalize(date.astimezone(timezone))
        duration = float(route[2])
        samples.append([(date - startDate).total_seconds() / 3600, duration / 60])
samples = numpy.array(samples)

# replace first column with index in half-hour increment
samples[:, 0] = numpy.round(2*samples[:, 0])

# reshape as array with 24*2 columns
rowCount = numpy.ceil(numpy.max(samples[:, 0]) / 48)
samplesPerDay = numpy.zeros((rowCount, 48))
for i in range(0, samples.shape[0]):
    index = samples[i, 0]
    samplesPerDay[numpy.floor(index / 48), index % 48] = samples[i, 1]

# plot array
plt.imshow(samplesPerDay[1:-1,:],interpolation='nearest')
plt.gray()
plt.show()
