import json
from os.path import exists

from confluent_kafka import Producer
from pathlib import Path
import socket
import os

conf=os.environ.get('BOOTSTRAP_SERVERS') #('BOOTSTRAP_SERVERS', 'broker:9092'(
print(f"the environment is: {conf}")
server_address={"bootstrap.servers":conf}
producer=Producer(server_address)
def result_of_send(err, msg):
    if err is not None:
        print("Failed to deliver message: %s: %s" % (str(msg), str(err)))
    else:
        print("Message produced: %s" % (str(msg)))

def producer_data_from_file(file_name):
    root_dir=Path(__file__).resolve().parent
    file=root_dir/"Data"/file_name
    try:
        with open(file,"r") as f:
            data=json.load(f)
            for content in range(0, len(data)):
                print(f"produce message number {content}")
                producer.produce("raw_data",value=json.dumps(data[content]),callback=result_of_send)
                producer.poll(0)
            producer.flush()
            print("flush successfully...")
    except FileNotFoundError:
        print(f"Error file {file} was not found")
    except json.JSONDecodeError as e:
        print(f"error: invalid json: {e}")
if __name__=="__main__":
    producer_data_from_file("field_reports.json")