
const { ethers } = require("ethers");
require("dotenv").config();
const {ABI } = require("./utils");
const INFURA_URL = process.env.INFURA_URL; // Sepolia RPC URL
const PRIVATE_KEY = process.env.PRIVATE_KEY; // Wallet private key

const contractAddress = "0x5b2a6640153D1df9b6a8bB37E9B07EBa9F06b637"; // Replace with your deployed contract address
const contractABI = ABI;

const interact = async () => {
  // Connect to Sepolia network
  const provider = new ethers.JsonRpcProvider(INFURA_URL);
  const wallet = new ethers.Wallet(PRIVATE_KEY, provider);
  const contract = new ethers.Contract(contractAddress, contractABI, wallet);

  // Example Interaction: Calling a getter function
  console.log("Reading storedData from the contract...");
  const storedData = await contract.adjustableInterestRateDetailsMap("12AB"); // Replace 'get' with your function name
  // const storedData = await contract.changeLender("0xF5cEA5e4B6126ffe06B2AEF2Ce8c1c3b981fA3F4"); // Replace 'get' with your function name
   //const storedData = await contract.setAdjustableInterestRateDetails("12AB","14/01/25","Noida","UP","XYZ","ABC"); // Replace 'get' with your function name
  console.log("Stored Data:", storedData);
};

interact().catch((error) => console.error(error.message));















  // Example Interaction: Calling a setter function
//   console.log("Setting new value in the contract...");
//   const tx = await contract.set(42); // Replace 'set' and parameters with your function name
//   await tx.wait();
//   console.log("Transaction hash:", tx.hash);

  // Verify updated value
//   const updatedData = await contract.get();
//   console.log("Updated Stored Data:", updatedData.toString());


