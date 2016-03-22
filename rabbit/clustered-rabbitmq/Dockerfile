FROM rabbitmq:3.6.1-management
MAINTAINER Arialdo Martini <arialdomartini@gmail.com>

COPY clusterize /usr/local/bin/
RUN chmod a+x /usr/local/bin/clusterize

EXPOSE 5672 15672 25672 4369 9100 9101 9102 9103 9104 9105
USER rabbitmq
CMD ["clusterize"]
