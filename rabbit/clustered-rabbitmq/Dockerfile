FROM rabbitmq:3.6.1-management
MAINTAINER Arialdo Martini <arialdomartini@gmail.com>

COPY clusterize /usr/local/bin/
RUN chmod a+x /usr/local/bin/clusterize

EXPOSE 5672 15672
USER rabbitmq
CMD ["clusterize"]
