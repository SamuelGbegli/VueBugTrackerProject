//Store for managing a user login to the app

import type UserDTO from "@/classes/DTOs/UserDTO";

import axios, { AxiosError } from "axios";
import { defineStore } from "pinia";
import { useRouter } from "vue-router";

const router = useRouter();

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
        const response = await axios.post("/auth/login", userDTO);

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
      return this.user != null;
    },

    //TODO: possibly remove
    //Function to verify the user's JWT
    async isJWTValid(){
      if (!this.user.token) return false;

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

    //Checks if the user is logged in with ASP.NET's system
    async isLoggedInBackend(){
      try{
        const response = await axios.get("auth/isloggedin");
        if(!response.data){
          this.user = null;
          localStorage.removeItem("user");
        }
        return (response.data);
      }
      catch (ex){
        //If the frontend cannot connect to the server, ensures that the user store is empty


        this.user = null;
        localStorage.removeItem("user");
        return false;
      }
    },

    //Function to log the user out
    async logout(){

      //Removes user values from stores
      console.log("logging out...");
      const response = await axios.post("/auth/logout", {});
      this.user = null;
      localStorage.removeItem("user");

      //Reloads page when logged in
      router.go(0);
    },
    //Gets the id of the user logged in
    getUserID(){
      if(!!this.user) return this.user.id;
      return "";
    }
  }
})
