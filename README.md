# ClawCreator

A claw-machine character creator made in Unity. You steer a claw over capsules on a conveyor belt and grab them. Each capsule holds an item (headwear, eyes, nose, mouth, outfit or ears) that gets put on the current character. After three grabs the character is done: it cheers and walks off into the crowd, and a polaroid with a QR code slides out. The QR code links to the finished character on https://clawsembly.vercel.app.

## Controls

- **Move the claw:** A/D, left/right arrows or the gamepad left stick
- **Drop / grab:** the activate input bound on the claw's `PlayerInput`

## How it works

- `GameManager` counts the grabs left and moves on to the next character.
- `Claw` animates the drop, grab and return with PrimeTween.
- `ItemCollector` puts a grabbed item on the current `Character` and refills the capsule from the `BallContainer`.
- `ItemSO` is a ScriptableObject that describes one item: its sprites, mesh, materials and `EquipmentType`.
- `FormationManager` and `CrowdFormation` arrange the finished characters into a crowd.
- `QrGenerator` builds the QR code with QRCoder (installed through NuGetForUnity).

## Getting started

1. Open the project in **Unity 6000.4.1f1** (URP).
2. Open `Assets/Scenes/GameScene.unity`. It is the only scene in the build settings.
3. Press Play.

## Credits

Made by David Märzendorfer, ElitoGame, PhilippW33 and KareemAL.

Third-party assets: [PrimeTween](https://github.com/KyryloKuzyk/PrimeTween), [EditorAttributes](https://github.com/v0lt13/EditorAttributes), [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) with QRCoder, Ciro Continisio's toon shader, the Gritline Toon Shader and Lana Studio's Hyper Casual FX.
