FROM python:3.13-slim
WORKDIR /data
COPY requirements.txt ./
COPY Producer.py .
COPY Data/ ./Data/
RUN pip install --no-cache-dir -r requirements.txt
ENTRYPOINT ["python","Producer.py"]