<b>Pré-requisitos</b>

Antes de começar, certifique-se de que sua máquina possui:

    -Docker Desktop instalado e a uma distribuição wsl linux instalada dentro do docker.

    -Git instalado (opcional, apenas se preferir clonar via SSH)

<b>Passo a Passo para Execução</b>

1. Iniciar o Docker Desktop
Abra o Docker Desktop e aguarde até que o status fique verde ("Docker Desktop is running")
2. Baixar o Projeto
   
    Opção 1: Via HTTPS (recomendado)
    git clone https://github.com/vinisouzans/MTU.git

    Opção 2: Via download direto
    Acesse https://github.com/vinisouzans/MTU, clique em "Code" > "Download ZIP"
    Extraia o arquivo ZIP em uma pasta de sua preferência
  
4. Abra um PowerShell como administrador
Acesse a pasta do projeto "cd MTU"
5.  Executar a Aplicação com o comando "docker-compose up --build'
6. Aguardar a Inicialização
O processo pode levar alguns minutos na primeira execução enquanto:
Baixa as imagens do .NET, PostgreSQL e RabbitMQ
Constrói a aplicação
Inicializa todos os containers
7. Verificar se Todos os Serviços Estão Rodando com o comando "docker-compose ps"
8. Acessar a Aplicação
   
    API e Swagger: http://localhost:5000/swagger
  
    RabbitMQ Management: http://localhost:15672
  
    Usuário: guest
  
    Senha: guest
