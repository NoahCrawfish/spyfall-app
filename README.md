# Spyfall App

An iOS application built in **Unity (C#)** based on the party game **Spyfall**. All players except the Spy receive the same location with unique roles. The Spy receives no location information and has to infer it through conversation while avoiding detection.

## Implementation Details

- **Purchasing + Monetization**
  - Integrated Unity IAP to support:
    - Ad-supported free play
    - Unlockable premium content

- **Persistent Data Storage**
  - Player settings are stored using Unity's built-in `PlayerPrefs`
  - Location pack data (including custom packs) is serialized to **XML**

- **Other Key Features**
  - Implemented a mini drawing tool for users to create thumbnails for custom locations
  - Dynamically generated UI elements have their properties tweened on page transition or user interaction

## Demo Video

https://github.com/user-attachments/assets/6cd09436-495b-4943-ba98-7b5da2178633
