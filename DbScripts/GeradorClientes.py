import random

nomes_base = [
    "João", "Maria", "Carlos", "Ana", "Pedro", "Fernanda", "Lucas", "Juliana",
    "Marcos", "Camila", "Rafael", "Beatriz", "Felipe", "Amanda", "Bruno", "Letícia",
    "Eduardo", "Larissa", "Rodrigo", "Patrícia", "Magno", "Millena", "Joaquin", "Marcell"
]

sobrenomes = [
    "Silva", "Souza", "Oliveira", "Santos", "Lima", "Pereira", "Costa", "Almeida",
    "Ferreira", "Rodrigues", "Martins", "Barbosa", "Dias", "Araujo", "Teixeira", "Fonseca", "Lopes", "Gonçalves"
]

for i in range(1, 101):
    nome = f"{random.choice(nomes_base)} {random.choice(sobrenomes)}"
    saldo = round(random.uniform(0, 1000000), 2)
    print(f"INSERT INTO Clientes (Nome, SaldoDisponivel) VALUES (N'{nome}', {saldo});")

