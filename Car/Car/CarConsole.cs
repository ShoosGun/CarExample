﻿using CarExample;
using System;
using UnityEngine;

namespace CarExample.Car
{
    internal class CarConsole : MonoBehaviour
    {
        public OWRigidbody carBody;

        private SingleInteractionVolume interactVolume;

        private PlayerAttachPoint attachPoint;

        private PlayerAudioController playerAudio;

        private ScreenPrompt drivePrompt;
        private ScreenPrompt steerPrompt;

        public CarWheelController carWheelController;

        private string drivePromptStr = "Drive";
        private string steerPromptStr = "Steer";

        public event Action OnEnterCarConsole;
        public event Action OnExitCarConsole;

        private void Awake()
        {
            enabled = false;

            drivePrompt = new ScreenPrompt(InputLibrary.thrustZ, drivePromptStr, 1);
            steerPrompt = new ScreenPrompt(InputLibrary.thrustX, steerPromptStr, 1);

            attachPoint = this.GetRequiredComponent<PlayerAttachPoint>();
            interactVolume = this.GetRequiredComponent<SingleInteractionVolume> ();
            playerAudio = Locator.GetPlayerAudioController();

            interactVolume.OnPressInteract += OnPressInteract;
        }

        private void OnDestroy()
        {
            interactVolume.OnPressInteract -= OnPressInteract;
        }
        private void OnPressInteract()
        {
            if (!enabled)
            {
                playerAudio.PlayOneShotInternal(AudioType.ShipCockpitBuckleUp);
                attachPoint.AttachPlayer();
                interactVolume.DisableInteraction();
                enabled = true;
                OnEnterCarConsole?.Invoke();

                Locator.GetPromptManager().AddScreenPrompt(drivePrompt, PromptPosition.LowerLeft, true);
                Locator.GetPromptManager().AddScreenPrompt(steerPrompt, PromptPosition.LowerLeft, true);
            }
        }

        private void Update()
        {
            if (OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.All))
            {
                attachPoint.DetachPlayer();
                playerAudio.PlayOneShotInternal(AudioType.ShipCockpitUnbuckle);
                interactVolume.EnableInteraction();
                interactVolume.ResetInteraction();
                enabled = false;
                OnExitCarConsole?.Invoke();


                Locator.GetPromptManager().RemoveScreenPrompt(drivePrompt);
                Locator.GetPromptManager().RemoveScreenPrompt(steerPrompt);
            }
        }
    }
}
