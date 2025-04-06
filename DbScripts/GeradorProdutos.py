import random
from datetime import datetime, timedelta

# Categorias com seus produtos típicos
categoria_produtos = {
    "Renda Fixa": ["CDB", "LCI", "LCA", "Letra Financeira", "Tesouro"],
    "Renda Variável": ["Fundo Ações", "ETF", "FOF"],
    "Crédito Privado": ["Debênture", "CRI", "CRA", "Nota Comercial"],
    "Fundos": ["Fundo Multimercado", "Fundo ESG", "Fundo Previdenciário"],
    "Tesouro Direto": ["Tesouro Prefixado", "Tesouro IPCA+"],
    "Cambial": ["Fundo Cambial"],
    "Previdência": ["Fundo Previdenciário"],
    "ESG": ["Fundo ESG", "Debênture Sustentável"],
    "Infraestrutura": ["Fundo Infraestrutura", "Debênture Incentivada"]
}

# Sufixos aleatórios
sufixos = [
    "Pós-Fixado", "Prefixado", "IPCA+", "Curto Prazo", "Longo Prazo", "Incentivada",
    "CDI 100%", "CDI 95%", "Isento IR", "Com Liquidez Diária", "Sem Liquidez",
    "Juros Semestrais", "FIC", "Sustentável"
]

# Gerar INSERTs para categorias com IDENTITY_INSERT
insert_categorias = ["SET IDENTITY_INSERT Categorias ON;"]
categoria_ids = {}
for idx, nome in enumerate(categoria_produtos.keys(), start=1):
    insert_categorias.append(f"INSERT INTO Categorias (Id, Nome) VALUES ({idx}, N'{nome}');")
    categoria_ids[nome] = idx
insert_categorias.append("SET IDENTITY_INSERT Categorias OFF;")

# Gerar produtos com vencimento entre 7 e 180 dias, sem inserir Id
produtos = []
for categoria, nomes_base in categoria_produtos.items():
    for _ in range(10):  # 10 produtos por categoria
        base = random.choice(nomes_base)
        sufixo = random.choice(sufixos)
        nome = f"{base} {sufixo}"

        preco = round(random.uniform(90, 30000), 2)
        quantidade = random.randint(5, 1000)
        dias_para_vencer = random.randint(7, 180)
        data_venc = datetime.now() + timedelta(days=dias_para_vencer)
        categoria_id = categoria_ids[categoria]

        insert = (
            f"INSERT INTO Produtos (Nome, Preco, QuantidadeEstoque, DataVencimento, CategoriaId) "
            f"VALUES (N'{nome}', {preco}, {quantidade}, '{data_venc.strftime('%Y-%m-%d')}', {categoria_id});"
        )
        produtos.append(insert)

# Montar script final
script_sql = "\n".join(insert_categorias + produtos)

# Exibir ou salvar
print(script_sql)

