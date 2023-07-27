# Transactions API

## Requisitos
- Docker Desktop

## Como rodar
Rode o comando baixo em um terminal bash ou similar.

```bash
export POSTGRES_PASSWORD=<senha do banco>   # pode ser omitido se a variável estiver nas variáveis de ambiente do sistema
docker-compose up -d
```

Após a primeira execução, a aplicação demorará cerca de 60 segundos para ficar acessível. Esse período de espera é necessário 
para que a aplicação espere o banco inicializar para então rodar a migração inicial do modelo. Para validar se terminou de rodar, 
faça uma requisição para http://localhost:8080/health-check. Será retornado um 200 OK caso esteja no ar.

## Como usar
A Swagger UI está habilitada para uso e pode ser acessada em http://localhost:8080/swagger. Podem ser feitas requisições diretamente
a API url cURL ou algum outro cliente HTTP.

## Limpeza
Para remover os recursos criados acima, use o comando `docker-compose down -v`. Esse comando deletará também o volume associado 
ao Postgres. Caso deseje apenas para os containers, omita a opção `-v`

## O que faltou
Não foi feita uma interface gráfica para interação com a API devido a questão de tempo. 
