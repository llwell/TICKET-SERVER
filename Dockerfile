FROM microsoft/dotnet
WORKDIR /app
EXPOSE 80
ADD Ticket-Server/obj/Docker/publish /app
RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
ENTRYPOINT ["dotnet", "Ticket_Server.dll"]
