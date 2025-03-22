//Store for managing a user login to the app

import type UserDTO from "@/classes/DTOs/UserDTO";

import axios, { AxiosError } from "axios";
import { defineStore } from "pinia";

export const useAuthStore = defineStore("auth", {
  //User value is user item fetched from localStorage
  state: () => ({
    user: JSON.parse(localStorage.getItem("user") || "{}")
  }),
  actions:{
    //Function to log the user in
    async login(userDTO: UserDTO){
      try{
        //Sends user DTO to backend server
        const response = await axios.post("/auth", userDTO);

        //Stores returned information if login was successful
        if(response.status === 200){
          const userInfo = response.data;
          this.user = userInfo;
          localStorage.setItem("user", JSON.stringify(userInfo));
          return response.status;
        }
        //Returns status code for frontend to handle
      }
      catch(ex){
        //Something went wrong, return error message

        let error = ex as AxiosError;

        //Message if client cannot connect to server
        if (error.status === 500)
          return ("Cannot connect to server. Please try again later.")

        //Message is client connected to server
        return (ex as AxiosError).request.response;
      }
    },

    //Checks if a user is logged in
    isLoggedIn() {
      return this.user != null && !!this.user.token;
    },

    //Function to verify the user's JWT
    async isJWTValid(){
      if (!this.user.token) return false;

      //TODO: add method to connect to backend and verify token
      try{
        //Sends token to backend to validate
        const response = await axios.post("/auth/validatetoken", JSON.parse(localStorage.getItem("user") || "{}").token,{
          headers: {"Content-Type": "application/json"}
        });

        //Only exists to test if the token is valid
          console.log("Token is valid");
      }
      catch (ex){

                //Logs the user out if token is invalid
                if((ex as AxiosError).status != 500){
                  console.log("Token is invalid");
                  this.logout();
                }
                else
        //Server error, means token cannot be validated
        console.log("Cannot connect to server.", ex);
      }
    },

    //Function to log the user out
    logout(){
      //Removes user values from stores
      console.log("logging out...");
      this.user = null;
      localStorage.removeItem("user");
    }
  }
})
