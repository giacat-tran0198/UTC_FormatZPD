<template>
    <div id="knowledge">
        <div v-if="dataReady" > 
        <TreeKnowledges ref="treeKnowledges" v-bind:show="show" v-bind:height = "'90%'"  v-bind:series="knowledges"/>
        </div>
        <button class="button" v-on:click="open_form('add')">Add knowledge</button>
        <button class="button" v-on:click="open_form('modify')">Modify knowledge</button>
        <button class="button" v-on:click="open_form('delete')">Delete knowledge</button>
        <form  v-if="openFormAddDelete" @submit.prevent="verify_form()">
            <p v-if="action_type=='add'">
                <label for="name">Name</label>
                <input
                id="name"
                v-model="name"
                type="text"
                name="name"
                >
            </p>

            <p v-if="action_type=='add'">
                <label for="parentName">Parent Name</label>
               <select v-model="parent">
                <option  v-for="knowledge in knowledges" :key="knowledge.id" currentName>{{knowledge.name}}</option>
              </select>
            </p>

            <p v-if="action_type=='add'" >
              <label for="type">Type</label>
              <select v-model="type">
                <option type>knowledge</option>
                <option>didactic</option>
              </select>
            </p>

            <p v-if="action_type=='delete'">
              <label for="nameToDelete">Knowledge to delete</label>
              <select v-model="name">
                <option  v-for="knowledge in knowledges" :key="knowledge.id" currentName>{{knowledge.name}}</option>
              </select>
            </p>

            <div>
             <button v-if="action_type=='add'" class="button-submit" type="button" v-on:click="add_knowledge()">Add</button>
             <button  v-if="action_type=='delete'" class="button-delete" type="button" v-on:click="confirm_delete_node()">Delete</button>
            </div>
        </form>

        <form v-if="openFormModify" @submit.prevent="verify_form()">
            <p>
            <label for="nameToModify">Knowledge to modify</label>
            <select v-model="currentName">
              <option  v-for="knowledge in knowledges" :key="knowledge.id" currentName>{{knowledge.name}}</option>
            </select>
            </p>
            <p>
                <label for="name">New Name</label>
                <input
                id="name"
                v-model="name"
                type="text"
                name="name"
                >
            </p>
            <p>
            <label for="type">New Type</label>
            <select v-model="type">
              <option type>knowledge</option>
              <option>didactic</option>
            </select>
            </p>
            <div>
             <button class="button-submit" type="submit" v-on:click="modify_knowledge(currentName)">Modify</button>
            </div>
        </form>
         <!-- POP UP POUR MISSING VALUES --> 
        <vs-dialog v-model="popupActive">
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
                      <vs-button block @click="popupActive=!popupActive" color="primary" typeof="filled">Close</vs-button>
                  </div>
              </template>
        </vs-dialog>
        <!-- POP UP POUR DELETE --> 
        <vs-dialog v-model="popupConfirmDelete">
              <template #header>
                  <h4 class="not-margin">
                      Delete node
                  </h4>
              </template>
              <div class="con-content">
                  <p>
                      Are you sure you want to delete {{name}}?
                  </p>
              </div>
              <template #footer>
                  <div class="footer-dialog">
                      <vs-button block @click="delete_knowledge_api()" color="success" typeof="filled">Yes</vs-button>
                      <vs-button block @click="popupConfirmDelete=!popupConfirmDelete" color="danger" typeof="filled">No</vs-button>
                  </div>
              </template>
        </vs-dialog>
    </div>

</template>

<script>
import TreeKnowledges from '../components/Trees/TreeKnowledges.vue'; 
import axios from 'axios'
axios.defaults.baseURL = '//localhost:5000/';
export default {
 name: 'knowledges',
 components: {
   TreeKnowledges, 
 },
 data() {
   return {
     show: true, //to show the update button in the tree component
     knowledges : [], //list of knowledges
     parent : null, //parent of node we want to add
     name : null, //name to add or to modify
     type : 'knowledge', //type of node we want to add
     action_type : '', //"modify", "add", "delete"
     openFormModify : false, //to open the modify form
     openFormAddDelete : false, //to open the add or delete form
     currentName: null, //name to delete
     dataReady: false, //api resquest ready
     popupActive : false, //to open missing values pop up
     popupConfirmDelete: false, //confirm if the user wants to delete a node

   }
 },
  async mounted() {
    try{
      const knowledges = await axios.get("api/nodes")
      for (let idx= 0; idx < knowledges.data.length; idx++){
          var parentID = ''
            if(!knowledges.data[idx].removed){
              if(knowledges.data[idx].parentId != '-1'){
                parentID = knowledges.data[idx].parentId
              }
              var knowledge_formated={
                id: knowledges.data[idx].id,
                name: knowledges.data[idx].title,
                parent : parentID, 
                sibling: '',
                cls: 'bwhite', 
                "data-type":''
              }
              this.knowledges.push(knowledge_formated)
              this.dataReady = true;
            }
      }
    }catch(error){
      console.log(error)
    }
}
, 
methods: {
  getknowledges: async function(){
  try{
  const knowledges = await axios.get("api/nodes")
  for (let idx= 0; idx < knowledges.length; idx++){
      var parentID = ''
        if(!knowledges[idx].removed){
        if(knowledges[idx].parentId != '-1'){
          parentID = knowledges[idx].parentId
        }
        var knowledge_formated={
          id: knowledges[idx].id,
          name: knowledges[idx].title,
          parent : parentID, 
          sibling: '',
          cls: 'bwhite', 
          "data-type":''
        }
          this.knowledges.push(knowledge_formated)
          this.dataReady = true;
      }}
  }catch(error){
    console.log(error)
  }},
    add_knowledge : async function(){
        try{
          if(this.verify_form("add")){
          var parentID = ''
          for (let idx = 0; idx < this.knowledges.length; idx++) {
                if(this.knowledges[idx].name == this.parent){
                    parentID = this.knowledges[idx].id
                    }
          }       
          const knowledge = {title:this.name, type: this.type, parentId: parentID}
          const response = await axios.post("api/nodes", knowledge)
          var parentid = ''
            if(response.data.parentId != '-1'){
              parentid = response.data.parentId
            }
            var knowledge_formated={
              id: response.data.id,
              name: response.data.title,
              parent : parentid, 
              sibling: '',
              cls: 'bwhite', 
              "data-type":response.data.type
            }
          
          this.knowledges.push(knowledge_formated)
          }
          else {
            this.popupActive=true
          }
        }
        catch (error) {
            console.error(error);
        }
        this.openFormAddDelete = false
        this.parent =null
        this.name = null
    }, 
      
    delete_knowledge_api : async function(){
      try{
        if(this.verify_form("delete")){
          var nodeId = ''
          for (let idx = 0; idx < this.knowledges.length; idx++) {
                  if(this.knowledges[idx].name == this.name){
                      nodeId = this.knowledges[idx].id
                      }
          }
          await axios.delete("api/nodes/"+nodeId)
          this.delete_knowledge(nodeId); 
          this.openFormAddDelete = false
          this.parent = null
          this.name = null
          this.popupConfirmDelete=false
          this.$refs.treeKnowledges.update_tree() 
        }else{
          this.popupActive=true
        }

      } catch(error) {
        console.log(error)
        }
    }, 
    delete_knowledge : function(nodeId){
        for (let idx = 0; idx < this.knowledges.length; idx++) {
            if(this.knowledges[idx].id == nodeId){
                this.knowledges.splice(idx, 1);
            }
        }
    }, 
    find_node_id : function (nodeName){
    for (let idx = 0; idx < this.knowledges.length; idx++) {
                if(this.knowledges[idx].name == nodeName){
                    return this.knowledges[idx].id
                }
    }
    },
    modify_knowledge : async function(currentName){
       try {
        if(this.verify_form("modify")){
            const currentid = this.find_node_id(currentName)
            const response = await axios.put("api/nodes/"+currentid, {title: this.name, type : this.type});
            for (let idx = 0; idx < this.knowledges.length; idx++) {
                    if(this.knowledges[idx].id == currentid){
                        this.knowledges[idx].name = response.data.title
                        }
            }
        }else{
          this.popupActive=true
        }
        } catch (error) {
        console.error(error);
      }
      this.openFormModify = false
      this.name=null
    },
    open_form : function(action_type){
      if(action_type =="add" | action_type =="delete" ){
        this.action_type = action_type
        this.openFormAddDelete=true
        this.openFormModify=false
      }
      else {
        this.action_type = action_type
        this.openFormAddDelete=false
        this.openFormModify=true
      }
    }, 
    verify_form : function(action_type){
      if(action_type=="add"){
        console.log("name is", this.name)
        console.log("parent is", this.parent)
        if(this.name!=null && this.parent!=null){
          console.log("je suis la?")
          return true
        }
        else {
          return false
        }
      }
      if(action_type=="modify"){
        if(this.name!=null && this.currentName!=null){
          return true
        }
        else {
          return false
        }
      }
      if(action_type=="delete"){
        if(this.name!=null){
          return true
        }
        else {
          return false
        }
      } 
    }, 
    confirm_delete_node : function(){
       this.popupConfirmDelete=true
    }
}
}
</script>
<style scoped >
body {
   font-family: 'Lato';
   height: 100%;
   background: rgb(253, 254, 255);
 }

 .button{
  display: inline-block;
  background: rgb(50,100,200)	;
  color : #fff; 
  border: 1px #0a1557;
  border-style: solid;
  padding-right: 8px; 
  padding-left: 8px;  
  padding-top : 5px; 
  padding-bottom: 5px; 
  margin: 2px;
  border-radius: 10px;
  cursor: pointer;
  text-decoration: none;
  font-size:15px;
  font-family:inherit; 
}
.button:hover{
  background: #0a1557;	 
}
.button:focus{
    background: #0a1557;
}
 .button-submit{
  display: inline-block;
  background: #3fb84ffb	;
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
.button-submit:hover{
  background: #306b09;	 
}
 .button-delete{
  display: inline-block;
  background: #d43636fb	;
  color : #fff; 
  border: 1px #6b0909;
  border-style: solid;
  padding: 5px 8px; 
  margin: 2px;
  border-radius: 8px;
  cursor: pointer;
  text-decoration: none;
  font-size:15px;
  font-family:inherit; 
}
.button-delete:hover{
  background: #6b0909;	 
}
label {
  display: inline-block;
  width: 200px;
  text-align: left;
  padding-right: 15px;
  font-size: 15px;
  font-weight:bold;
}
select {
  text-align: left;
  display: inline-block;
  width: 150px;
  height : 25px;
  background-color: rgba(128, 128, 128, 0.226); 
  border : none;  
  border-radius : 40px;
  padding-left: 10px
}
select:focus {
  border-style: solid;
  border-width: 1px;
  border-color:rgba(0, 68, 255, 0.89);
}
input {
  display: inline-block;
  width: 150px;
  height : 25px;
  background-color: rgba(128, 128, 128, 0.226); 
  border : none;  
  border-radius : 40px;
  padding-left: 10px
}

input:focus {
  border-style: solid;
  border-width: 1px;
  border-color:rgba(0, 68, 255, 0.89);
}
option{
  font-size: 15px;
  text-align: left;
}


</style>
