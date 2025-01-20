const { ethers } = require("ethers");
require("dotenv").config();
const { bytecode, ABI } = require("./utils");
const INFURA_URL = process.env.INFURA_URL; // Sepolia Infura URL
const PRIVATE_KEY = process.env.PRIVATE_KEY; // Wallet private key

const contractABI = ABI;
const contractBytecode = bytecode; // Paste your contract's bytecode

const deploy = async () => {
  try {
    const provider = new ethers.JsonRpcProvider(INFURA_URL);
    const wallet = new ethers.Wallet(PRIVATE_KEY, provider);

    console.log("Deploying the contract to Sepolia...");

    const factory = new ethers.ContractFactory(
      contractABI,
      contractBytecode,
      wallet
    );
    const contract = await factory.deploy();

    console.log("Contract deployed at:", contract.target);
  } catch (e) {
    console.log("the error is ", e);
  }
  //   console.log("Transaction hash:", contract.deployTransaction.hash);
};

deploy().catch((error) => console.error(error));
