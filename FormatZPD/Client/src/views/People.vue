<template>
 <div class="people">
    <!-- TITRE + BOUTON ADD-->
    <div class="nav">
        <div class="item0">
          <h1>Student List</h1>
        </div>
        <div class="item1" >
          <button class="button" v-on:click="open_form_add()">Add Student</button>
        </div>
    </div>
    <!-- LA LISTE DES ETUDIANTS-->
      <div class="liste">
        <tr>
          <button class="boutonliste" href="#" variant="info" v-for="person in people" :key="person.id" >
              {{ person.name}}  {{person.lastname}}
            <div class="option">
            <button class="button" v-on:click="open_zpd_profil(person.id)">ZPD Profil</button>
            <button class="button" v-on:click="open_form_modify(person.id, person.name, person.lastname)">Modify</button>
            <button class="button" v-on:click="confirm_delete_student(person.id)">Remove</button>
            </div>
          </button>
        </tr>
      </div>
      <!-- POP UP POUR AJOUT  OU MODIFICATION D'UN APPRENANT--> 
      <div class="centerx">
      <vs-dialog v-model="addDialogActiv">
            <template #header>
                <h4 class="not-margin">
                    {{ !openFormModify?"Add":"Modify" }} a student
                </h4>
            </template>
            <div class="con-form">
                <vs-input  class="vs-input" placeholder="First Name" label="First Name" v-model="personFirstName" />
                <vs-input   class="vs-input" placeholder="Last Name" label="Last Name" v-model="personLastName" />
            </div>
            <template #footer>
                <div class="footer-dialog">
                    <vs-button block @click="add_student()" v-show="openFormAdd" color="primary" typeof="filled">Add</vs-button>
                    <vs-button block @click="modify_student()" v-show="openFormModify" color="primary" typeof="filled">Modify</vs-button>
                </div>
            </template>
        </vs-dialog>

      </div>
      <!-- POP UP POUR MISSING VALUES --> 
      <vs-dialog v-model="popupActivo">
            <template #header>
                <h4 class="not-margin">
                    Missing values
                </h4>
            </template>
            <div class="con-content">
                <p>
                    All fields are required.
                </p>
            </div>
            <template #footer>
                <div class="footer-dialog">
                    <vs-button block @click="popupActivo=!popupActivo" color="primary" typeof="filled">Close</vs-button>
                </div>
            </template>
      </vs-dialog>
      <!-- POP UP POUR DELETE --> 
      <vs-dialog v-model="confirmDelete">
            <template #header>
                <h4 class="not-margin">
                    Delete student
                </h4>
            </template>
            <div class="con-content">
                <p>
                    Are you sure you want to delete {{personFirstName}} {{personLastName}} ?
                </p>
            </div>
            <template #footer>
                <div class="footer-dialog">
                    <vs-button block @click="delete_student_api()" color="success" typeof="filled">Yes</vs-button>
                     <vs-button block @click="confirmDelete=!confirmDelete" color="danger" typeof="filled">No</vs-button>
                </div>
            </template>
      </vs-dialog>
    </div> 
</template>


<script>
import axios from 'axios'
axios.defaults.baseURL = '//localhost:5000/';
export default {
 name: 'people',
 components: {
 },
 data() {
   return {
     openFormAdd : false,
     openFormModify : false, 
     people : [],
     personid : '', 
     personLastName : '', 
     personFirstName: '', 
     popupActivo : false, 
     confirmDelete: false, 
     addDialogActiv: false
   }
 },
 mounted : function(){
  axios.get("api/people")
  .then(response => {
        response.data.forEach(person => {
          console.log(person.removed)
          if(person.removed==false){
          var person_formated={
            id: person.id,
            name: person.firstName,
            lastname : person.lastName,
            removed : person.removed
          }
          this.people.push(person_formated)
          this.people.sort(function compare(a, b) {
                const nameA = a.name.toUpperCase();
                const nameB = b.name.toUpperCase();
                if (nameA < nameB)
                  return -1;
                if (nameA > nameB)
                  return 1;
                return 0;
            });
          }

    })})
  .catch(error => console.log(error)); 
 },

 methods: {
     open_zpd_profil : function(personid){
        this.$router.push( '/zpdprofil' )
        this.personid = personid
        this.$store.commit('change', personid)
        console.log(this.$store.state.student)
     }, 
     add_student : async function() {
      try {
        if(this.verify_form()){
        const person = {firstname:this.personFirstName, lastname: this.personLastName}
        const response = await axios.post("api/people", person);
          if(response.data.removed==false){
            var person_formated={
              id: response.data.id,
              name: response.data.firstName,
              lastname : response.data.lastName,
              removed : response.data.removed
            }
            console.log(this.people.push(person_formated));
            this.people.sort(function compare(a, b) {
                const nameA = a.name.toUpperCase();
                const nameB = b.name.toUpperCase();
                if (nameA < nameB)
                  return -1;
                if (nameA > nameB)
                  return 1;
                return 0;
            });
          }
        } else{
          this.popupActivo=true
        } 
      } catch (error) {
        console.error(error);
      }
      this.addDialogActiv=false
      this.openFormAdd= false
      this.personFirstName =''
      this.personLastName = ''
    }, 
    delete_student_api : async function(){ 
        try {
        console.log(this.personid)
        await axios.delete("api/people/"+this.personid);
        this.delete_student(this.personid);
        this.confirmDelete=false
        } catch (error) {
        console.error(error);
      }
    },
    delete_student : function(personID){
        console.log("id de la personne", personID)
        for (let idx = 0; idx < this.people.length; idx++) {
                if(this.people[idx].id == personID){
                    this.people.splice(idx, 1)
                    }
        }
    },
    confirm_delete_student : function(personId){
      this.confirmDelete=true
      this.personid=personId
      const student =this.people.find(student => {
        return student.id == personId
      })
      this.personLastName=student.lastname
      this.personFirstName=student.name
    },
    modify_student : async function(){
       try {
        if(this.verify_form()){
        const response = await axios.put("api/people/"+this.personid, {firstName: this.personFirstName , lastName : this.personLastName });
        for (let idx = 0; idx < this.people.length; idx++) {
                if(this.people[idx].id == this.personid){
                    this.people[idx].name = response.data.firstName
                    this.people[idx].lastname = response.data.lastName
                    this.people[idx].removed = response.data.removed
                    }
        }
        this.people.sort(function compare(a, b) {
                const nameA = a.name.toUpperCase();
                const nameB = b.name.toUpperCase();
                if (nameA < nameB)
                  return -1;
                if (nameA > nameB)
                  return 1;
                return 0;
            });
        }
        else {
          this.popupActivo=true
        }
  
        } catch (error) {
        console.error(error);
      }
      this.addDialogActiv=false
      this.openFormModify = false
      this.personFirstName =''
      this.personLastName = ''
    }, 
    open_form_modify : function(personid, personFirstName, personLastName){
      this.personFirstName = personFirstName
      this.personLastName = personLastName
      this.personid = personid
      this.openFormModify = true
      this.openFormAdd = false
      this.addDialogActiv=true
    }, 
    open_form_add : function(){
      this.personFirstName =''
      this.personLastName = ''
      this.openFormAdd = true
      this.openFormModify = false
      this.addDialogActiv=true
    }, 

    openAlert: function(){
      this.$vs.dialog({
        type:'confirm',
        color: 'danger',
        title: `Missing values`,
        text: 'All fileds are required.',
      })
    }, 
    verify_form : function() {
      if(this.personFirstName.length && this.personLastName.length){
        return true
      }
      else {
        return false
        }
    },
    filter_list : function(){
      
    }

 }
}
</script>

<style scoped>
html {
  scroll-behavior: smooth;
}

.item0 {
  display: inline-block;
  line-height: 1;
  width:57%;
  text-align: right;
}

.item1{
  display: inline-block;
  vertical-align:top; 
  width:43%;
}
.button{
  display: inline-block;
  background: #a8a8a8	;
  color : #fff; 
  border: 1px #6e6e6e;
  border-style: solid;
  padding: 5px 8px; 
  margin: 2px;
  border-radius: 8px;
  cursor: pointer;
  text-decoration: none;
  font-size:15px;
  font-family:inherit; 
}
.button:hover{
  background: #5a5959;	 
}
.boutonliste{
  display: inline-block;
  background: #d1eaf3	;
  color : rgb(85, 85, 85); 
  border: 1px #9dc3f3;
  border-style: solid;
  padding: 5px 8px; 
  margin: 2px;
  border-radius: 0px;
  cursor: pointer;
  text-decoration: none;
  font-size: x-large;
  font-family:inherit; 
  width: 1000px;
  text-align: center;
  
}
.boutonliste:hover{
  background: #41b1fcc4;	 
}
.boutonliste:focus{
  background-color: #41b1fcc4;	 
}
.liste{
  align-content: start;
  align-content: space-between; 
  justify-content: left;
}
.button:hover{
  background: #5a5959;	 
}

.con-form {
    width: 600px;
}

.con-form .vs-input-content {
  margin: 10px;
  width: calc(100%)
}

.con-form .vs-input {
  width: 580px;
  height: 35px;
  margin-bottom : 20px; 
  margin-bottom : 20px; 
  border-radius : 50px; 
}

.con-content {
  width: 100%;
}

.con-content p {
  font-size: .8rem;
  padding: 0px 10px;
}


</style>