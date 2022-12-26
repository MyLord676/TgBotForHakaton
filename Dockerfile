FROM python:3.9-alpine
LABEL version="1.0"

RUN mkdir /usr/src/DjangoTgAdmin
WORKDIR /usr/src/DjangoTgAdmin

RUN pip install --upgrade pip

COPY ./requirements.txt ./requirements.txt

RUN pip install -r ./requirements.txt

CMD ["python", "./taskmanager/manage.py", "runserver", "0.0.0.0:8080"]
