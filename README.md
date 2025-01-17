# Gestion Supermarché - Application .NET MAUI

## Description
Application mobile et desktop développée avec .NET MAUI permettant de gérer les heures travaillées des employés dans différents rayons d'un supermarché.

## Fonctionnalités

### 1. Gestion des Données
- Création/modification/supprestion des employés
- Création/modification/supprestion des rayons
- Création/modification/supprestion des secteurs

### 2. Gestion du Temps de Travail
- Saisie des heures travaillées par employé et par rayon
- Validation des données (un employé ne peut pas travailler dans le même rayon à la même date)
- Date par défaut = date du jour

### 3. Consultation
- Visualisation des temps de travail par employé
- Affichage détaillé : rayon, date, nombre d'heures
- Total des heures par employé

### 4. Statistiques
- Nombre d'heures par employé
- Total des heures par rayon
- Total général des heures
- Total des heures par mois

### 5. Visualisations
- Graphiques variés (histogrammes,barre de progression, camemberts)
- Visualisation des données statistiques

### 6. Converter  
- Un Converter pour la coloration des employés dans **GestionEmployé**, permettant de savoir si une personne a effectué ses 8 heures de travail aujourd'hui.  
- Un Converter pour s'assurer que la personne saisit un maximum de 24 heures, sans accepter de valeur décimale (pas de virgule).  

## Prérequis Techniques

### Packages NuGet Utilisés

#### Base de Données
- `sqlite-net-pcl` : ORM pour SQLite
- `SQLitePCLRaw.bundle_green` : Provider SQLite

#### Graphiques
- `OxyPlot.Maui.Skia` : Bibliothèque de visualisation de données avancée permettant de créer des graphiques interactifs et personnalisables. Compatible avec .NET MAUI via le moteur de rendu SkiaSharp.
- `SkiaSharp` : Moteur de rendu graphique 2D cross-platform utilisé comme base pour OxyPlot. Offre des performances optimales pour le dessin de formes, courbes et graphiques.

#### Interface Utilisateur
- `CommunityToolkit.Maui` : Composants UI additionnels

### Configuration

1. Cloner le repository
2. Ouvrir la solution dans Visual Studio 2022
3. Restaurer les packages NuGet
4. Compiler et exécuter l'application

## Architecture du Projet

```
GestionSupermarche/
├── DTO/                  # Classes utilitaire
├── Models/               # Classes de données
├── Pages/                # Pages XAML
├── Platforms/            # Configuration par plateforme
├── Repositories/         # Accès aux données
├── Resources/            # Ressources de l'application
├── Services/             # Services (BDD, etc.)
└── Converter/            # mise en forme special   
```

## Base de Données

Structure des tables :
- Employe (IdEmploye, Nom)
- Secteur (IdSecteur, Nom)
- Rayon (IdRayon, Nom, IdSecteur)
- TempsTravail (IdTempsTravail, IdEmploye, IdRayon, Date, HeuresTravaillees)
