const abi = [
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "_borrower",
				"type": "address"
			}
		],
		"name": "approveLoan",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
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
		"inputs": [
			{
				"internalType": "uint256",
				"name": "_penaltyRate",
				"type": "uint256"
			}
		],
		"stateMutability": "nonpayable",
		"type": "constructor"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "borrower",
				"type": "address"
			}
		],
		"name": "BorrowerRemoved",
		"type": "event"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "_penaltyRate",
				"type": "uint256"
			}
		],
		"name": "changePenaltyRate",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
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
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "borrower",
				"type": "address"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "emiAmount",
				"type": "uint256"
			}
		],
		"name": "EMIPayment",
		"type": "event"
	},
	{
		"inputs": [],
		"name": "fullRepayment",
		"outputs": [],
		"stateMutability": "payable",
		"type": "function"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "borrower",
				"type": "address"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "amount",
				"type": "uint256"
			}
		],
		"name": "FullRepayment",
		"type": "event"
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
				"internalType": "uint256",
				"name": "loanId",
				"type": "uint256"
			}
		],
		"name": "GeneralDetailsUpdated",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
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
		"name": "LoanFunded",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "borrower",
				"type": "address"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "amount",
				"type": "uint256"
			}
		],
		"name": "LoanRepaid",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "borrower",
				"type": "address"
			}
		],
		"name": "LoanRequestRejected",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "borrower",
				"type": "address"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "amount",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "interestRate",
				"type": "uint256"
			}
		],
		"name": "LoanRequested",
		"type": "event"
	},
	{
		"inputs": [],
		"name": "payEMI",
		"outputs": [],
		"stateMutability": "payable",
		"type": "function"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "loanId",
				"type": "uint256"
			}
		],
		"name": "PaymentDetailsUpdated",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "borrower",
				"type": "address"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "penaltyAmount",
				"type": "uint256"
			}
		],
		"name": "PenaltyApplied",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "lender",
				"type": "address"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "panaltyRate",
				"type": "uint256"
			}
		],
		"name": "panaltyRatechanged",
		"type": "event"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "_loanAmount",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "_interestRate",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "_repaymentDurationInMonths",
				"type": "uint256"
			}
		],
		"name": "requestLoan",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "loanId",
				"type": "uint256"
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
				"internalType": "uint256",
				"name": "_principalAmount",
				"type": "uint256"
			},
			{
				"internalType": "string",
				"name": "_lenderName",
				"type": "string"
			},
			{
				"internalType": "uint256",
				"name": "_initialInterestRate",
				"type": "uint256"
			},
			{
				"internalType": "string",
				"name": "_changeDate",
				"type": "string"
			},
			{
				"internalType": "uint256",
				"name": "_maxRateOnFirstChange",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "_minRateOnFirstChange",
				"type": "uint256"
			}
		],
		"name": "setGeneralDetails",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "loanId",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "_initialMonthlyPayment",
				"type": "uint256"
			},
			{
				"internalType": "string",
				"name": "_firstPaymentDate",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "_maturityDate",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "_paymentAddress",
				"type": "string"
			},
			{
				"internalType": "uint256",
				"name": "_gracePeriod",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "_lateChargePercentage",
				"type": "uint256"
			},
			{
				"internalType": "string",
				"name": "_index",
				"type": "string"
			},
			{
				"internalType": "uint256",
				"name": "_margin",
				"type": "uint256"
			},
			{
				"internalType": "bool",
				"name": "_allowsPrepayment",
				"type": "bool"
			},
			{
				"internalType": "uint256",
				"name": "_defaultNoticePeriod",
				"type": "uint256"
			}
		],
		"name": "setPaymentDetails",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "takeOutContractFunds",
		"outputs": [],
		"stateMutability": "nonpayable",
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
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"name": "generalDetailsMap",
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
				"internalType": "uint256",
				"name": "principalAmount",
				"type": "uint256"
			},
			{
				"internalType": "string",
				"name": "lenderName",
				"type": "string"
			},
			{
				"internalType": "uint256",
				"name": "initialInterestRate",
				"type": "uint256"
			},
			{
				"internalType": "string",
				"name": "changeDate",
				"type": "string"
			},
			{
				"internalType": "uint256",
				"name": "maxRateOnFirstChange",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "minRateOnFirstChange",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "getActiveBorrowers",
		"outputs": [
			{
				"internalType": "address[]",
				"name": "",
				"type": "address[]"
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
		"name": "getBorrowerCount",
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
				"internalType": "uint256",
				"name": "loanId",
				"type": "uint256"
			}
		],
		"name": "getPayPlusGeneralDetails",
		"outputs": [
			{
				"components": [
					{
						"internalType": "uint256",
						"name": "initialMonthlyPayment",
						"type": "uint256"
					},
					{
						"internalType": "string",
						"name": "firstPaymentDate",
						"type": "string"
					},
					{
						"internalType": "string",
						"name": "maturityDate",
						"type": "string"
					},
					{
						"internalType": "string",
						"name": "paymentAddress",
						"type": "string"
					},
					{
						"internalType": "uint256",
						"name": "gracePeriod",
						"type": "uint256"
					},
					{
						"internalType": "uint256",
						"name": "lateChargePercentage",
						"type": "uint256"
					},
					{
						"internalType": "string",
						"name": "index",
						"type": "string"
					},
					{
						"internalType": "uint256",
						"name": "margin",
						"type": "uint256"
					},
					{
						"internalType": "bool",
						"name": "allowsPrepayment",
						"type": "bool"
					},
					{
						"internalType": "uint256",
						"name": "defaultNoticePeriod",
						"type": "uint256"
					}
				],
				"internalType": "struct MultiLoanContract.PaymentDetails",
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
						"internalType": "uint256",
						"name": "principalAmount",
						"type": "uint256"
					},
					{
						"internalType": "string",
						"name": "lenderName",
						"type": "string"
					},
					{
						"internalType": "uint256",
						"name": "initialInterestRate",
						"type": "uint256"
					},
					{
						"internalType": "string",
						"name": "changeDate",
						"type": "string"
					},
					{
						"internalType": "uint256",
						"name": "maxRateOnFirstChange",
						"type": "uint256"
					},
					{
						"internalType": "uint256",
						"name": "minRateOnFirstChange",
						"type": "uint256"
					}
				],
				"internalType": "struct MultiLoanContract.GeneralDetails",
				"name": "",
				"type": "tuple"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "getRequestedBorrowers",
		"outputs": [
			{
				"internalType": "address[]",
				"name": "",
				"type": "address[]"
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
		"name": "isRepaymentOverdue",
		"outputs": [
			{
				"internalType": "bool",
				"name": "",
				"type": "bool"
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
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"name": "loans",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "loanAmount",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "interestRate",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "totalRepayment",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "remainingRepayment",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "emiAmount",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "emiCount",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "emisPaid",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "nextEmiDueDate",
				"type": "uint256"
			},
			{
				"internalType": "bool",
				"name": "isLoanFunded",
				"type": "bool"
			},
			{
				"internalType": "bool",
				"name": "isLoanRepaid",
				"type": "bool"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"name": "paymentDetailsMap",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "initialMonthlyPayment",
				"type": "uint256"
			},
			{
				"internalType": "string",
				"name": "firstPaymentDate",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "maturityDate",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "paymentAddress",
				"type": "string"
			},
			{
				"internalType": "uint256",
				"name": "gracePeriod",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "lateChargePercentage",
				"type": "uint256"
			},
			{
				"internalType": "string",
				"name": "index",
				"type": "string"
			},
			{
				"internalType": "uint256",
				"name": "margin",
				"type": "uint256"
			},
			{
				"internalType": "bool",
				"name": "allowsPrepayment",
				"type": "bool"
			},
			{
				"internalType": "uint256",
				"name": "defaultNoticePeriod",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "penaltyRate",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	}
]

export default abi;
