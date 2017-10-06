import sqlite3
from sqlite3 import Error as SqliteError
import os

def createConnection(dbFile):
    try:
        conn = sqlite3.connect(dbFile)
        return conn
    except SqliteError as error:
        print(error)

    return None;

def selectAllRows(conn):
    cur = conn.cursor()
    cur.execute("SELECT * FROM Training_Data")

    rows = cur.fetchall()

    for row in rows:
        print(row)

def main():
    database = os.path.abspath(os.path.join(os.path.dirname( __file__ ), '..', 'asl_data.db'))
    conn = createConnection(database)
    selectAllRows(conn)

if __name__ == '__main__':
    main()