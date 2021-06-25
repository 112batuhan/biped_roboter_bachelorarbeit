import os

class Logger:

    def __init__(self, name ,path = r"D:\User\Desktop\tez\ML\algorithm\tmp\logs"):

        self.path = path
        self.name = name

    def log(self, data, mode="a"):

        line = ",".join(str(e) for e in data)
        line += "\n"

        complete_path = os.path.join(self.path, self.name)
        file = open(complete_path, mode)
        file.write(line)
        file.close()
    
    def first_line(self,data):

        self.log(data, "w")