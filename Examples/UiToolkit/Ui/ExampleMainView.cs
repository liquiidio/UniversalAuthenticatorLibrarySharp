using System;
using Assets.Packages.UniversalAuthenticatorLibrary.Src.UiToolkit.Ui;
using System.Collections.Generic;
using System.Threading.Tasks;
using EosSharp.Core.Api.v1;
using UnityEngine;
using UnityEngine.UIElements;
using Action = EosSharp.Core.Api.v1.Action;

namespace Assets.Packages.UniversalAuthenticatorLibrary.Examples.UiToolkit.Ui
{
    public class ExampleMainView : ScreenBase
    {
        /*
         * Child-Controls
         */
        private Button _changeToBidNameButton;
        private Button _changeToBuyRamButton;
        private Button _changeToSellRamButton;
        private Button _changeToTransferButton;
        private Button _changeToVoteButton;

        private Button _bidButton;
        private Button _buyRamButton;
        private Button _transferTokenButton;
        private Button _voteButton;
        private Button _sellRamButton;
        private Button _logoutButton;

        private TextField _toTextField;
        private TextField _fromTextField;
        private TextField _memoTextField;
        private TextField _nameToBidTextField;
        private TextField _quantityTextField;
        private TextField _receiverAccountTextField;
        private TextField _userAccountTextField;
        private TextField _sellRamAmountTextField;
        private TextField _amountToBuyTextField;
        private TextField _amountWaxTextField;
        private TextField _bidAmountTextField;

        private VisualElement _sellRamBox;
        private VisualElement _transferTokenBox;
        private VisualElement _bidNameBox;
        private VisualElement _buyRamBox;
        private VisualElement _voteBox;

        private Label _accountLabel;

        /*
         * Fields, Properties
         */
        [SerializeField] internal UALUiToolkitExample UALUiToolkitExample;

        private User _user;


        private void Start()
        {
            _changeToTransferButton = Root.Q<Button>("change-to-transfer-button");
            _changeToVoteButton = Root.Q<Button>("change-to-vote-button");
            _changeToSellRamButton = Root.Q<Button>("change-to-sell-ram-button");
            _changeToBuyRamButton = Root.Q<Button>("change-to-buy-ram-button");
            _changeToBidNameButton = Root.Q<Button>("change-to-bid-button");

            _transferTokenButton = Root.Q<Button>("transfer-token-button");
            _voteButton = Root.Q<Button>("vote-button");
            _sellRamButton = Root.Q<Button>("sell-ram-button");
            _buyRamButton = Root.Q<Button>("buy-ram-button");
            _bidButton = Root.Q<Button>("bid-button");
            _logoutButton = Root.Q<Button>("log-out-button");

            _accountLabel = Root.Q<Label>("account-label");

            _toTextField = Root.Q<TextField>("to-account-text-field");
            _fromTextField = Root.Q<TextField>("from-account-text-field");
            _memoTextField = Root.Q<TextField>("memo-text-field");
            _quantityTextField = Root.Q<TextField>("quantity-text-field");
            _userAccountTextField = Root.Q<TextField>("user-account");
            _amountWaxTextField = Root.Q<TextField>("amount-text-field");
            _nameToBidTextField = Root.Q<TextField>("name-to-bid-text-field");
            _bidAmountTextField = Root.Q<TextField>("bid-amount-text-field");
            _sellRamAmountTextField = Root.Q<TextField>("sell-amount-text-field");
            _receiverAccountTextField = Root.Q<TextField>("receiver-account-text-field");
            _amountToBuyTextField = Root.Q<TextField>("amount-to-buy-text-field");

            _transferTokenBox = Root.Q<VisualElement>("transfer-token-box");
            _voteBox = Root.Q<VisualElement>("vote-box");
            _sellRamBox = Root.Q<VisualElement>("sell-ram-box");
            _buyRamBox = Root.Q<VisualElement>("buy-ram-box");
            _bidNameBox = Root.Q<VisualElement>("bid-name-box");

            UALUiToolkitExample.UnityUiToolkitUal.OnUserLogin += Rebind;

            BindButtons();
            SetTransferAccountText();
            SetSellRamText();
            SetBuyRamText();
            SetBidNameText();
        }

        #region Button Binding

        private void BindButtons()
        {
            _changeToTransferButton.clickable.clicked += () =>
            {
                _transferTokenBox.Show();
                _voteBox.Hide();
                _sellRamBox.Hide();
                _buyRamBox.Hide();
                _bidNameBox.Hide();
            };

            _changeToVoteButton.clickable.clicked += () =>
            {
                _transferTokenBox.Hide();
                _voteBox.Show();
                _sellRamBox.Hide();
                _buyRamBox.Hide();
                _bidNameBox.Hide();
            };

            _changeToSellRamButton.clickable.clicked += () =>
            {
                _transferTokenBox.Hide();
                _voteBox.Hide();
                _sellRamBox.Show();
                _buyRamBox.Hide();
                _bidNameBox.Hide();
            };

            _changeToBuyRamButton.clickable.clicked += () =>
            {
                _transferTokenBox.Hide();
                _voteBox.Hide();
                _sellRamBox.Hide();
                _buyRamBox.Show();
                _bidNameBox.Hide();
            };

            _changeToBidNameButton.clickable.clicked += () =>
            {
                _transferTokenBox.Hide();
                _voteBox.Hide();
                _sellRamBox.Hide();
                _buyRamBox.Hide();
                _bidNameBox.Show();
            };

            _transferTokenButton.clickable.clicked += async () =>
            {
                var actor =  UALUiToolkitExample.User.GetAccountName().Result;

                var action = new Action
                {
                    account = "eosio.token",
                    name = "transfer",
                    authorization = new List<PermissionLevel> 
                    {
                        new() 
                        {
                            actor =
                                "............1", // ............1 will be resolved to the signing accounts permission
                            permission =
                                "............2" // ............2 will be resolved to the signing accounts authority
                        }
                    },

                    data = new Dictionary<string, object>
                    {
                        { "from", actor },
                        { "to", _toTextField.value },
                        { "quantity", _quantityTextField.value },
                        { "memo", _memoTextField.value }
                    }
                };
                try
                {
                    await UALUiToolkitExample.Transact(action);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    throw;
                }
            };

            _voteButton.clickable.clicked += async () =>
            {
                var producers = new List<string> { "liquidstudio" };

                var action = new Action
                {
                    account = "eosio",
                    name = "voteproducer",
                    authorization = new List<PermissionLevel>
                    {
                        new()
                        {
                            actor =
                                "............1", // ............1 will be resolved to the signing accounts permission
                            permission =
                                "............2" // ............2 will be resolved to the signing accounts authority
                        }
                    },
                    data = new Dictionary<string, object>
                    {
                        { "voter", "............1" },
                        { "proxy", "coredevproxy" },
                        { "producers", producers.ToArray() }
                    }
                };

                try
                {
                    await UALUiToolkitExample.Transact(action);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    throw;
                }
            };

            _buyRamButton.clickable.clicked += async () =>
            {
                var action = new Action
                {
                    account = "eosio",
                    name = "buyram",
                    authorization = new List<PermissionLevel>
                    {
                        new()
                        {
                            actor =
                                "............1", // ............1 will be resolved to the signing accounts permission
                            permission =
                                "............2" // ............2 will be resolved to the signing accounts authority
                        }
                    },
                    data = new Dictionary<string, object>
                    {
                        { "payer", "............1" },
                        { "quant", _amountToBuyTextField.value },
                        { "receiver", _receiverAccountTextField.value }
                    }
                };
                try
                {
                    await UALUiToolkitExample.Transact(action);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    throw;
                }
            };

            _sellRamButton.clickable.clicked += async () =>
            {
                var action = new Action
                {
                    account = "eosio",
                    name = "sellram",

                    authorization = new List<PermissionLevel>
                    {
                        new()
                        {
                            actor =
                                "............1", // ............1 will be resolved to the signing accounts permission
                            permission =
                                "............2" // ............2 will be resolved to the signing accounts authority
                        }
                    },
                    data = new Dictionary<string, object>
                    {
                        { "account", "............1" },
                        { "bytes", _sellRamAmountTextField.value }
                    }
                };
                try
                {
                    await UALUiToolkitExample.Transact(action);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    throw;
                }
            };

            _bidButton.clickable.clicked += async () =>
            {
                var actor = await UALUiToolkitExample.User.GetAccountName();

                var action = new Action
                {
                    account = "eosio",
                    name = "bidname",

                    authorization = new List<PermissionLevel>
                    {
                        new()
                        {
                            actor =
                                "............1", // ............1 will be resolved to the signing accounts permission
                            permission =
                                "............2" // ............2 will be resolved to the signing accounts authority
                        }
                    },
                    data = new Dictionary<string, object>
                    {
                        { "newname", _nameToBidTextField.value },
                        { "bidder", actor },
                        { "bid", _bidAmountTextField.value }
                    }
                };
                try
                {
                    await UALUiToolkitExample.Transact(action);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    throw;
                }
            };

            _logoutButton.clickable.clicked += () =>
            {
                try
                {
                    UALUiToolkitExample.UnityUiToolkitUal.Init();
                    Hide();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    throw;
                }
            };
        }

        #endregion

        #region Rebind
        private async void Rebind(User user)
        {
            _user = user;
            var accountName = await _user.GetAccountName();

            _fromTextField.value = accountName;
            _accountLabel.text = accountName;
            _receiverAccountTextField.value = accountName;

            this.Show();
        }

        #endregion

        #region other
        private void SetTransferAccountText()
        {
            var toName = "???";
            var memoComment = "Anchor is the best! Thank you.";
            var quantityAmount = "0.00001000 WAX";

            _toTextField.SetValueWithoutNotify(toName);
            _memoTextField.SetValueWithoutNotify(memoComment);
            _quantityTextField.SetValueWithoutNotify(quantityAmount);
        }

        private void SetSellRamText()
        {
            var amount = "0";

            _sellRamAmountTextField.SetValueWithoutNotify(amount);
        }

        private void SetBuyRamText()
        {
            var name = "";
            var quantityAmount = "0.00001000 WAX";

            _receiverAccountTextField.SetValueWithoutNotify(name);
            _amountToBuyTextField.SetValueWithoutNotify(quantityAmount);
        }

        private void SetBidNameText()
        {
            var name = "new name";
            var amount = "0.00010000 WAX";

            _nameToBidTextField.SetValueWithoutNotify(name);
            _bidAmountTextField.SetValueWithoutNotify($"{amount}");
        }

        #endregion
    }
}