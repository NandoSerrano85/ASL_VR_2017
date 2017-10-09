import sqlite3
from sqlite3 import Error as SqliteError
import os
import json

class Database:
    __connection = None
    __NUM_FEATURES = 7

    def __init__(self, databaseFile, tableName):
        self.path_to_db_file = databaseFile
        self.tableName = tableName

    def createConnection(self):
        if Database.__connection is None:
            try:
                Database.__connection = sqlite3.connect(self.path_to_db_file)
            except SqliteError as error:
                print("Couldn't connect to the database: " + error)

    def returnCursor(self):
        return Database.__connection.cursor()

    def selectAllRows(self, TableName):
        cur = self.returnCursor()
        return cur.execute("SELECT * FROM " + self.tableName)

    def insertToRow(self, features):
        if len(features) != Database.__NUM_FEATURES:
            raise ValueError("Expected " + str(Database.__NUM_FEATURES) + " features, got " + str(len(features)))

        cur = self.returnCursor()
        cur.execute("INSERT INTO " + self.tableName + " VALUES(?, ?, ?, ?, ?, ?, ?)", features)
        Database.__connection.commit()
