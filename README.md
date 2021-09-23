# FormatZPD

Le sujet de thèse traite de l’entraînement d’apprenants dans des situations stressantes dans le domaine de la médecine
militaire réalisée dans des contextes particulièrement ardus

## Membre du projet

- Claire Taupin
- Quoc Gia Cat Tran
- Juliette Défossez
- Maria Al Bejjani

## Exécuter l'interface du client Vuejs

- Ouvrir le terminal
- Allez au dossier Client
  ```
  cd ./client
  ```
- Installez les package:
  ```
  yarn install
  ```
  ou
  ```
  npm install
  ```
- Excutez:
  ```
  yarn serve
  ```
  ou
  ```
  npm run serve
  ```
- Ouvrez le navigateur: `localhost:8080`

## Excuter ce serveur

### Visual Studio

- Appuyez sur `F5`

### Terminal

- `dotnet run`

### Installer le APOC dans Neo4j

- lien: [APOC](https://github.com/neo4j-contrib/neo4j-apoc-procedures)

## Liste des uri d'api

### Pour les noeuds

- `parentId` est égal à `-1` ou alors on ne l'ajoute pas dans le format de publication, c'est-à-dire que ce noeud n'a
  pas de parent.
- Tous les noeuds:
    - méthode: `GET`
    - port: `api/nodes`
    - résultat:
  ```json
    [
        {
            "id": "024f7266-e4c1-43e8-80c1-daf5dfac14d8",
            "title": "Root",
            "type": "root",
            "parentId": "-1",
            "removed": false
        },
        {
            "id": "08ef5ec5-6bb9-4eec-a9c6-92d91c8d2886",
            "title": "Storm",
            "type": "didactic",
            "parentId": "806076ac-964e-4ca6-90e1-5d40f81b729f",
            "removed": false
        },
        {
            "id": "56d9fd71-74df-43ed-bb0b-6a593bc3b1da",
            "title": "Job Stress",
            "type": "knowledge",
            "parentId": "024f7266-e4c1-43e8-80c1-daf5dfac14d8",
            "removed": false
        }
    ]
  ```
    - Erreur:
        - Erreur lors de l'obtention de la liste des noeuds

- Trouver un noeud:
    - méthode: `GET`
    - port: `api/nodes/{id}`
    - résultat: `api/nodes/024f7266-e4c1-43e8-80c1-daf5dfac14d8`
  ```json
    {
      "id": "024f7266-e4c1-43e8-80c1-daf5dfac14d8",
      "title": "Root",
      "type": "root",
      "parentId": "-1",
      "removed": false
    }
  ```
    - Erreur:
        - Pas trouvé ce nœud
        - Erreur lors de l'obtention ce nœud

- Créer un nouveau noeud:
    - méthode: `POST`
    - port: `api/nodes`
    - format pour publier:
  ```json
  {
    "title": "Job Stress",
    "type": "knowledge",
    "parentId": "024f7266-e4c1-43e8-80c1-daf5dfac14d8"
  }
  ```
    - résultat:
  ```json
    {
      "id": "779da22d-7f7e-409f-8fb2-0bc92d4792cd",
      "title": "Job Stress",
      "type": "knowledge",
      "parentId": "024f7266-e4c1-43e8-80c1-daf5dfac14d8",
      "removed": false
    }
  ```
    - Erreur:
        - Pas trouvé le parent de ce nœud
        - Le parent a été supprimé
        - Erreur lors de la création de ce nœud

- Modifier un noeud:
    - méthode: `PUT`
    - port: `api/nodes/{id}`
    - format de publication: `api/nodes/779da22d-7f7e-409f-8fb2-0bc92d4792cd`
  ```json
    {
      "title": "Job Stress test edit",
      "type": "knowledge"
    }
  ```
    - résultat:
  ```json
    {
      "id": "779da22d-7f7e-409f-8fb2-0bc92d4792cd",
      "title": "Job Stress test edit",
      "type": "knowledge",
      "parentId": "779da22d-7f7e-409f-8fb2-0bc92d4792cd",
      "removed": false
    }
  ```
    - Erreur:
        - Le noeud à modifier n'existe pas
        - La personne à modifier a été supprimé
        - Erreur lors de la mis à jour ce nœud

- Supprimer un noeud :
    - Si ce noeud a des fils, leurs propriété `removed` va passer à `true`
        - ex: un noeud
          ````json
          {
          "id": "7aaaa1d2-176e-459e-a3c3-3c7d982663be",
          "title": "Acute Stress",
          "type": "knowledge",
          "parentId": "024f7266-e4c1-43e8-80c1-daf5dfac14d8",
          "removed": false
          }
          ````
        - résultat:
          ````json
          {
          "id": "7aaaa1d2-176e-459e-a3c3-3c7d982663be",
          "title": "Acute Stress",
          "type": "knowledge",
          "parentId": "024f7266-e4c1-43e8-80c1-daf5dfac14d8",
          "removed": true
          }
          ````
    - Si non, ce noeud est supprimé.
        - ex: un nouveau noeud
      ````json
      {
      "id": "40bb9bcb-429a-4a45-8113-fb9a70ee3ebb",
      "title": "Job Stress",
      "type": "knowledge",
      "parentId": "024f7266-e4c1-43e8-80c1-daf5dfac14d8",
      "removed": false
      }
      ````
        - `DELETE`: `api/nodes/40bb9bcb-429a-4a45-8113-fb9a70ee3ebb`
        - Resultat: `"La suppression ce nœud"`
    - Erreur:
        - Le noeud n'existe pas
        - Impossible de supprimer le nœud racine
        - Impossible de supprimer ce nœud car il a déjà été supprimé
        - Erreur lors de la suppression de ce nœud

### Pour les personnes

- Tous les personnes:
    - méthode: `GET`
    - port: `api/people`
    - résultat:
  ````json
    [
      {
          "id": "4b1d0c8d-9142-41b8-9028-2145b66609fc",
          "firstName": "Juliette",
          "lastName": "Défossez",
          "removed": false
      },
      {
          "id": "3634aa1a-293e-4e3c-889f-85ee5d798ff6",
          "firstName": "Maria",
          "lastName": "Bejjani",
          "removed": false
      },
      {
          "id": "228c64f7-79ed-456f-8457-e119030000d5",
          "firstName": "Claire Cindy",
          "lastName": "Taupin",
          "removed": false
      }
    ]
    ````
    - Erreur:
        - Erreur lors de l'obtention de la liste des personnes

- Trouver une personne:
    - méthode: `GET`
    - port: `api/people/{id}`
    - résultat: `api/people/228c64f7-79ed-456f-8457-e119030000d5`
  ````json
  {
    "id": "228c64f7-79ed-456f-8457-e119030000d5",
    "firstName": "Claire Cindy",
    "lastName": "Taupin",
    "removed": false
  }
  ````
    - Erreur:
        - Pas trouvé cette personne
        - Erreur lors de l'obtention cette personne

- Créer un nouveau noeud:
    - méthode: `POST`
    - port: `api/people`
    - format pour publier:
  ```json
  {
    "firstname": "Quoc Gia Cat",
    "lastname": "Tran"
  }
  ```
    - résultat:
  ```json
    {
      "id": "49f593a9-65cb-488f-9f08-f103a7afb672",
      "firstName": "Quoc Gia Cat",
      "lastName": "Tran",
      "removed": false
    }
  ```
    - Erreur:
        - Erreur lors de la création de cette personne

- Modifier une personne:
    - méthode: `PUT`
    - port: `api/people/{id}`
    - format de publication: `api/people/49f593a9-65cb-488f-9f08-f103a7afb672`
    ```json
      {
        "firstname": "Gia Cat",
        "lastname": "Tran"
      }
    ```
    - résultat:
    ```json
      {
        "id": "49f593a9-65cb-488f-9f08-f103a7afb672",
        "firstName": "Gia Cat",
        "lastName": "Tran",
        "removed": false
      }
    ```
    - Erreur:
        - La personne à modifier n'existe pas
        - La personne à modifier a été supprimé
        - Erreur lors de la mis à jour cette personne

- Supprimer une personne:
    - méthode: DELETE
    - port: api/people/{id}
    - Si cette personne a des believes, leurs propriété `removed` va passer à `true`
      - ex: un personne
      ````json
      {
        "id": "49f593a9-65cb-488f-9f08-f103a7afb672",
        "firstName": "Gia Cat",
        "lastName": "Tran",
        "removed": false
      }
      ````
      - résultat:
      ````json
      {
        "id": "49f593a9-65cb-488f-9f08-f103a7afb672",
        "firstName": "Gia Cat",
        "lastName": "Tran",
        "removed": true
      }
      ````
        - Si non, cette personne est supprimé.
            - ex: un nouveau noeud
          ````json
              {
                "id": "49f593a9-65cb-488f-9f08-f103a7afb672",
                "firstName": "Gia Cat",
                "lastName": "Tran",
                "removed": false
              }
          ````
        - `DELETE`: `api/people/49f593a9-65cb-488f-9f08-f103a7afb672`
            - Resultat: `"La personne a bien été supprimé"`
        - Erreur:
            - La personne n'existe pas
            - Impossible de supprimer cette personne car elle a déjà été supprimé
            - Erreur lors de la suppression de cette personne

### Pour les believes de personne

- Toutes les personnes:
    - méthode: `GET`
    - port: `api/people/{personId}/believes`
    - résultat:
  ````json
    [
    {
        "id": "0cb81b21-00c6-45f9-8a19-24091a8b1ee9",
        "hability": 0.7,
        "dishability": 0.2,
        "ignorance": 0,
        "conflict": 0.1,
        "personId": "0626ca85-94ac-4e7a-a805-374e09d2e612",
        "colorBelief": "ff00b200",
        "interactions": [
            {
                "nodeId": "e0ddb97c-e058-47cf-86d7-777d36a9444b",
                "level": "medium"
            },
            {
                "nodeId": "bc0769e3-998c-4fc1-bed0-f152a49e5f34",
                "level": "strong"
            }
        ]
    },
    {
        "id": "3ee0befe-cc08-4b66-83ce-3fed6606c968",
        "hability": 0.8,
        "dishability": 0,
        "ignorance": 0,
        "conflict": 0.2,
        "personId": "0626ca85-94ac-4e7a-a805-374e09d2e612",
        "colorBelief": "ff00cc00",
        "interactions": [
            {
                "nodeId": "d8011585-6388-4334-a31e-9c6681a2bfdb",
                "level": "strong"
            },
            {
                "nodeId": "3984db18-48ad-46c1-8ecb-e37505c59bc2",
                "level": "medium"
            }
        ]
    },
    {
        "id": "256fddc0-4b6c-4bab-afb0-a1d43106a383",
        "hability": 0.8,
        "dishability": 0.1,
        "ignorance": 0,
        "conflict": 0.1,
        "personId": "0626ca85-94ac-4e7a-a805-374e09d2e612",
        "colorBelief": "ff00cc00",
        "interactions": [
            {
                "nodeId": "30319ea1-75af-4262-b631-2715ff015528",
                "level": "weak"
            },
            {
                "nodeId": "f312fc9c-19e9-45bc-8151-53324d624e75",
                "level": "weak"
            }
        ]
    }
  ]
    ````
    - Erreur:
        - La personne n'existe pas
        - Erreur lors de l'obtention des believes de la personne

- Trouver un belief:
    - méthode: `GET`
    - port: `api/people/believes/{beliefId}`
    - résultat:
  ````json
     {
        "id": "0cb81b21-00c6-45f9-8a19-24091a8b1ee9",
        "hability": 0.7,
        "dishability": 0.2,
        "ignorance": 0,
        "conflict": 0.1,
        "personId": "0626ca85-94ac-4e7a-a805-374e09d2e612",
        "colorBelief": "ff00b200",
        "interactions": [
            {
                "nodeId": "e0ddb97c-e058-47cf-86d7-777d36a9444b",
                "level": "medium"
            },
            {
                "nodeId": "bc0769e3-998c-4fc1-bed0-f152a49e5f34",
                "level": "strong"
            }
        ]
    }
    ````
    - Erreur:
        - Erreur lors de l'obtention de la croyance

- Creer/Modifier un belief:
    - méthode: `POST`
    - port: `api/people/{personId}/believes`
    - format de publication:
      ````json
      {
        "hability": 0.1,
        "dishability": 0.2,
        "ignorance": 0.6,
        "conflict": 0.1,
        "interactions": [
          {
          "nodeId": "148fbe5c-56d4-4cf5-8f61-3b313c08f117",
          "level": "medium"
          },
          {
          "nodeId": "14b5e82e-076a-429d-8f30-d43591339da2",
          "level": "weak"
          }
        ]
      }
      ````
    - résultat:
      ````json
        {
    "id": "07875f87-acd2-4589-8e45-898e9d9c1630",
    "hability": 0.1,
    "dishability": 0.2,
    "ignorance": 0.6,
    "conflict": 0.1,
    "personId": "3e8960e1-7142-4d56-94b8-57795a405f9a",
    "colorBelief": "Color [A=255, R=126, G=126, B=126]",
    "interactions": [
        {
            "nodeId": "148fbe5c-56d4-4cf5-8f61-3b313c08f117",
            "level": "medium"
        },
        {
            "nodeId": "14b5e82e-076a-429d-8f30-d43591339da2",
            "level": "weak"
        }
      ]
    }
      ````
        - erreur:
	    - La somme des variables doit être égale à 1
            - La personne avec ID=.. n'existe pas
            - La personne avec ID=.. a été supprimé
            - Le noeud avec ID=... n'existe pas
            - Le noeud avec ID=.. a été supprimé
            - Erreur lors de la création des believes de la personne (peut arriver si on donne deux noeuds parents qui ne sont pas de type 'didactic'


- Supprimer un belief:
    - méthode: `DELETE`
    - port: `api/people/believes/{beliefId}`
    - Résultat : Le belief a bien été supprimée
    - Erreur:
        - Le belief n'existe pas
        - Erreur lors de la suppression de le belief



- Récupérer une masse de croyance
    - méthode : `GET`
    - port: `api/people/{personId}/believes/parent/{parentId1}/{parentId2}`
    - résultat : 
     ````json
       {
    "id": "93625afc-aa9e-403e-adab-9d56d809cb69",
    "hability": 0.18,
    "dishability": 0.1,
    "ignorance": 0,
    "conflict": 0.72,
    "personId": "3e8960e1-7142-4d56-94b8-57795a405f9a",
    "colorBelief": "Color [A=255, R=183, G=0, B=0]",
    "interactions": [
        {
            "nodeId": "14b5e82e-076a-429d-8f30-d43591339da2",
            "level": "medium"
        }
      ]
    }
       ````
	- erreur : 
	   - La personne n'existe pas
	   - Erreur lors de l'obtention des believes de la personne avec ces noeuds parents