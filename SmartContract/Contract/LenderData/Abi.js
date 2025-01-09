const abi =[
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "_address",
				"type": "address"
			}
		],
		"name": "changeLender",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [],
		"stateMutability": "nonpayable",
		"type": "constructor"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "address",
				"name": "lender",
				"type": "address"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "amount",
				"type": "uint256"
			}
		],
		"name": "ContractFunded",
		"type": "event"
	},
	{
		"inputs": [
			{
				"internalType": "string",
				"name": "loanId",
				"type": "string"
			}
		],
		"name": "deleteLoan",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "fundContract",
		"outputs": [],
		"stateMutability": "payable",
		"type": "function"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "string",
				"name": "loanId",
				"type": "string"
			},
			{
				"indexed": false,
				"internalType": "string",
				"name": "propertyAddress",
				"type": "string"
			}
		],
		"name": "LoanCreated",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "string",
				"name": "loanId",
				"type": "string"
			}
		],
		"name": "LoanDeleted",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "string",
				"name": "loanId",
				"type": "string"
			},
			{
				"indexed": false,
				"internalType": "string",
				"name": "propertyAddress",
				"type": "string"
			}
		],
		"name": "LoanUpdated",
		"type": "event"
	},
	{
		"inputs": [
			{
				"internalType": "string",
				"name": "loanId",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "_noteDate",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "_city",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "_state",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "_propertyAddress",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "_paymentLocation",
				"type": "string"
			}
		],
		"name": "setAdjustableInterestRateDetails",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "string",
				"name": "loanId",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "_emiPaymentStartDate",
				"type": "string"
			},
			{
				"internalType": "uint256",
				"name": "_principalAmount",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "_fixedInterestRate",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "_monthlyPaymentAmount",
				"type": "uint256"
			},
			{
				"internalType": "string",
				"name": "_maturityDate",
				"type": "string"
			},
			{
				"internalType": "uint8",
				"name": "_emiPaymentDay",
				"type": "uint8"
			},
			{
				"internalType": "string",
				"name": "_interestRateChangeDate",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "_lenderName",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "_borrowerName",
				"type": "string"
			}
		],
		"name": "setCommonAndInterestDetails",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "string",
				"name": "loanId",
				"type": "string"
			},
			{
				"internalType": "uint8",
				"name": "_latePaymentGracePeriod",
				"type": "uint8"
			},
			{
				"internalType": "uint256",
				"name": "_lateChargePercentage",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "_margin",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "_currentIndex",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "_maxInterestRateAtFirstChange",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "_minInterestRateAtFirstChange",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "_maxInterestRateAfterChange",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "_minInterestRateAfterChange",
				"type": "uint256"
			}
		],
		"name": "setPaymentAndLateDetails",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "_amount",
				"type": "uint256"
			}
		],
		"name": "takeOutContractFunds",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "string",
				"name": "",
				"type": "string"
			}
		],
		"name": "adjustableInterestRateDetailsMap",
		"outputs": [
			{
				"internalType": "string",
				"name": "noteDate",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "city",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "state",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "propertyAddress",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "paymentLocation",
				"type": "string"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "checkBalanceOfSmartContract",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "string",
				"name": "",
				"type": "string"
			}
		],
		"name": "commonAndInterestDetailsMap",
		"outputs": [
			{
				"internalType": "string",
				"name": "emiPaymentStartDate",
				"type": "string"
			},
			{
				"internalType": "uint256",
				"name": "principalAmount",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "fixedInterestRate",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "monthlyPaymentAmount",
				"type": "uint256"
			},
			{
				"internalType": "string",
				"name": "maturityDate",
				"type": "string"
			},
			{
				"internalType": "uint8",
				"name": "emiPaymentDay",
				"type": "uint8"
			},
			{
				"internalType": "string",
				"name": "interestRateChangeDate",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "lenderName",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "borrowerName",
				"type": "string"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "getAllLoanIds",
		"outputs": [
			{
				"internalType": "string[]",
				"name": "",
				"type": "string[]"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "_borrower",
				"type": "address"
			}
		],
		"name": "getBorrowerBalance",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "getLenderBalance",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "string",
				"name": "loanId",
				"type": "string"
			}
		],
		"name": "getLoanDetails",
		"outputs": [
			{
				"components": [
					{
						"internalType": "string",
						"name": "emiPaymentStartDate",
						"type": "string"
					},
					{
						"internalType": "uint256",
						"name": "principalAmount",
						"type": "uint256"
					},
					{
						"internalType": "uint256",
						"name": "fixedInterestRate",
						"type": "uint256"
					},
					{
						"internalType": "uint256",
						"name": "monthlyPaymentAmount",
						"type": "uint256"
					},
					{
						"internalType": "string",
						"name": "maturityDate",
						"type": "string"
					},
					{
						"internalType": "uint8",
						"name": "emiPaymentDay",
						"type": "uint8"
					},
					{
						"internalType": "string",
						"name": "interestRateChangeDate",
						"type": "string"
					},
					{
						"internalType": "string",
						"name": "lenderName",
						"type": "string"
					},
					{
						"internalType": "string",
						"name": "borrowerName",
						"type": "string"
					}
				],
				"internalType": "struct LoanAgreementContract.CommonAndInterestDetails",
				"name": "",
				"type": "tuple"
			},
			{
				"components": [
					{
						"internalType": "uint8",
						"name": "latePaymentGracePeriod",
						"type": "uint8"
					},
					{
						"internalType": "uint256",
						"name": "lateChargePercentage",
						"type": "uint256"
					},
					{
						"internalType": "uint256",
						"name": "margin",
						"type": "uint256"
					},
					{
						"internalType": "uint256",
						"name": "currentIndex",
						"type": "uint256"
					},
					{
						"internalType": "uint256",
						"name": "maxInterestRateAtFirstChange",
						"type": "uint256"
					},
					{
						"internalType": "uint256",
						"name": "minInterestRateAtFirstChange",
						"type": "uint256"
					},
					{
						"internalType": "uint256",
						"name": "maxInterestRateAfterChange",
						"type": "uint256"
					},
					{
						"internalType": "uint256",
						"name": "minInterestRateAfterChange",
						"type": "uint256"
					}
				],
				"internalType": "struct LoanAgreementContract.PaymentAndLateDetails",
				"name": "",
				"type": "tuple"
			},
			{
				"components": [
					{
						"internalType": "string",
						"name": "noteDate",
						"type": "string"
					},
					{
						"internalType": "string",
						"name": "city",
						"type": "string"
					},
					{
						"internalType": "string",
						"name": "state",
						"type": "string"
					},
					{
						"internalType": "string",
						"name": "propertyAddress",
						"type": "string"
					},
					{
						"internalType": "string",
						"name": "paymentLocation",
						"type": "string"
					}
				],
				"internalType": "struct LoanAgreementContract.AdjustableInterestRateDetails",
				"name": "",
				"type": "tuple"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "lender",
		"outputs": [
			{
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "string",
				"name": "",
				"type": "string"
			}
		],
		"name": "paymentAndLateDetailsMap",
		"outputs": [
			{
				"internalType": "uint8",
				"name": "latePaymentGracePeriod",
				"type": "uint8"
			},
			{
				"internalType": "uint256",
				"name": "lateChargePercentage",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "margin",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "currentIndex",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "maxInterestRateAtFirstChange",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "minInterestRateAtFirstChange",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "maxInterestRateAfterChange",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "minInterestRateAfterChange",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	}
];
export default abi;
