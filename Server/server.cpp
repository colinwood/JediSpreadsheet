#include <stdio.h>
#include <unistd.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h> 
#include <sys/socket.h>
#include <netinet/in.h>

void accept_callback(int); 

void error(const char *msg)
{
    perror(msg);
    exit(1);
}


int main(int argc, char *argv[])
{
     int sockfd;    
     int newsockfd; 
     int port_number; //Port number to listening on
     int pid; //id of the process that is forked when a client connects
     socklen_t clilen;
     struct sockaddr_in serv_addr, cli_addr;

     if (argc < 2) {
         fprintf(stderr,"ERROR, no port provided\n");
         exit(1);
     }

     //Open the socket
     sockfd = socket(AF_INET, SOCK_STREAM, 0);
     if (sockfd < 0) 
        error("ERROR opening socket");

     bzero((char *) &serv_addr, sizeof(serv_addr)); //Zero out the buffer
     port_number = atoi(argv[1]); // convert the port to an int

     serv_addr.sin_family = AF_INET;
     serv_addr.sin_addr.s_addr = INADDR_ANY;
     serv_addr.sin_port = htons(port_number);

     //bind the socket
     if (bind(sockfd, (struct sockaddr *) &serv_addr,
              sizeof(serv_addr)) < 0) 
              error("ERROR on binding");
     
     listen(sockfd,5); //LIsten for a connection
     clilen = sizeof(cli_addr); //Get the size of the client ip address
     
     //Loop that connects clients as many as we need
     while (1) {

         newsockfd = accept(sockfd, 
               (struct sockaddr *) &cli_addr, &clilen);
         if (newsockfd < 0) 
             error("ERROR on accept");
         
         pid = fork(); // Start a new process so we can accept additional clients
         if (pid < 0)
             error("ERROR on fork");

         if (pid == 0)  {
             close(sockfd);
             accept_callback(newsockfd);
             exit(0);
         }
         else close(newsockfd);
     } 
     
     close(sockfd);
     return 0; 

}

/*
Called every time a client is connected send a confirmation everytime a message is received
Ideally this is where the server will parse the commands being received
*/
void accept_callback (int sock)
{
   fflush(stdout);
   printf("Client connected\n"); 
   
   int n;
   char message[256]; //Set up a buffer for reading from the socket
   bzero(message,256); //Clear out the buffer
   
   n = read(sock,message,255); //Read from the socket
   
   if (n < 0) 
   error("ERROR reading from socket");

   printf("Message from client: %s\n",message);

   n = write(sock,"Holy guacamole jedi you sent me a message!",18); //send a response back to client
   if (n < 0) error("ERROR writing to socket");
}