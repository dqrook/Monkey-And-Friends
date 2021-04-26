// GENERATED AUTOMATICALLY FROM 'Assets/Ryzm/Scripts/Player.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Ryzm
{
    public class @Player : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @Player()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player"",
    ""maps"": [
        {
            ""name"": ""PlayerMain"",
            ""id"": ""12b7cc40-589c-4e45-8799-37ae6fdbd23a"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""aa156a7c-f80d-4e74-b81a-a8923fd65b9a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""65895c68-7fe9-4336-912e-28e4cb0d8dfd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""51bbc4d7-79e2-484e-bee8-5789a6955046"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""7dd19565-775c-4101-bea0-433cfee2a642"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6b0a1f9f-4519-475d-acc9-36d6806f1925"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""c2169680-101c-46b7-8b69-d94eccd59847"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e7576dd7-8bc1-4442-bd57-08a5f479f61d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d910e234-4f56-41cc-bd01-fa20ada9a806"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""3392f95e-9cbc-4b81-867d-5b650e7742a8"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f47500a6-4d74-48e2-9bb6-f27acf3242c1"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4db0fa4d-2092-4db7-b80e-74325238a452"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d65f3952-1005-41ba-9ef2-d06edc16ffae"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6ea0cb6a-e821-4d5f-bdc6-4f3ec18cc66d"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""859e1541-2ea9-4b01-b409-9dae6ae3f7a3"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7904e4f3-85a5-4259-b315-9b9f524abc22"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // PlayerMain
            m_PlayerMain = asset.FindActionMap("PlayerMain", throwIfNotFound: true);
            m_PlayerMain_Move = m_PlayerMain.FindAction("Move", throwIfNotFound: true);
            m_PlayerMain_Jump = m_PlayerMain.FindAction("Jump", throwIfNotFound: true);
            m_PlayerMain_Look = m_PlayerMain.FindAction("Look", throwIfNotFound: true);
            m_PlayerMain_Attack = m_PlayerMain.FindAction("Attack", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // PlayerMain
        private readonly InputActionMap m_PlayerMain;
        private IPlayerMainActions m_PlayerMainActionsCallbackInterface;
        private readonly InputAction m_PlayerMain_Move;
        private readonly InputAction m_PlayerMain_Jump;
        private readonly InputAction m_PlayerMain_Look;
        private readonly InputAction m_PlayerMain_Attack;
        public struct PlayerMainActions
        {
            private @Player m_Wrapper;
            public PlayerMainActions(@Player wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_PlayerMain_Move;
            public InputAction @Jump => m_Wrapper.m_PlayerMain_Jump;
            public InputAction @Look => m_Wrapper.m_PlayerMain_Look;
            public InputAction @Attack => m_Wrapper.m_PlayerMain_Attack;
            public InputActionMap Get() { return m_Wrapper.m_PlayerMain; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerMainActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerMainActions instance)
            {
                if (m_Wrapper.m_PlayerMainActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_PlayerMainActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_PlayerMainActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_PlayerMainActionsCallbackInterface.OnMove;
                    @Jump.started -= m_Wrapper.m_PlayerMainActionsCallbackInterface.OnJump;
                    @Jump.performed -= m_Wrapper.m_PlayerMainActionsCallbackInterface.OnJump;
                    @Jump.canceled -= m_Wrapper.m_PlayerMainActionsCallbackInterface.OnJump;
                    @Look.started -= m_Wrapper.m_PlayerMainActionsCallbackInterface.OnLook;
                    @Look.performed -= m_Wrapper.m_PlayerMainActionsCallbackInterface.OnLook;
                    @Look.canceled -= m_Wrapper.m_PlayerMainActionsCallbackInterface.OnLook;
                    @Attack.started -= m_Wrapper.m_PlayerMainActionsCallbackInterface.OnAttack;
                    @Attack.performed -= m_Wrapper.m_PlayerMainActionsCallbackInterface.OnAttack;
                    @Attack.canceled -= m_Wrapper.m_PlayerMainActionsCallbackInterface.OnAttack;
                }
                m_Wrapper.m_PlayerMainActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Jump.started += instance.OnJump;
                    @Jump.performed += instance.OnJump;
                    @Jump.canceled += instance.OnJump;
                    @Look.started += instance.OnLook;
                    @Look.performed += instance.OnLook;
                    @Look.canceled += instance.OnLook;
                    @Attack.started += instance.OnAttack;
                    @Attack.performed += instance.OnAttack;
                    @Attack.canceled += instance.OnAttack;
                }
            }
        }
        public PlayerMainActions @PlayerMain => new PlayerMainActions(this);
        public interface IPlayerMainActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnLook(InputAction.CallbackContext context);
            void OnAttack(InputAction.CallbackContext context);
        }
    }
}
