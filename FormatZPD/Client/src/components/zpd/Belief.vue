<template>
    <td class="belief" v-bind:style="{ background: originalBelief.color}" @click="openDialog()">
        <vs-dialog v-model="addDialogActiv">
            <template #header>
                <h4 class="not-margin">
                    {{ (originalBelief.id==-1)?"Add":"Edit" }} a belief
                </h4>
            </template>
            <div class="con-content">
                <p>
                    Parent 1 : {{ parent1.name!=null?parent1.name:"Error" }} ({{  level1 != null ? level1 : "Error" }})
                </p>
                <p>
                    Parent 2 : {{ parent2.name!=null?parent2.name:"Error" }} ({{ level2 != null ? level2 : "Error" }})
                </p>
            </div>
            <div class="con-form">
                <vs-input type="number" min="0" max="1" step="0.1" placeholder="" label="Ability" v-model="originalBelief.ability" />
                <vs-input type="number" min="0" max="1" step="0.1" placeholder="" label="Disability" v-model="originalBelief.disability" />
                <vs-input type="number" min="0" max="1" step="0.1" placeholder="" label="Conflict" v-model="originalBelief.conflict" />
                <vs-input type="number" min="0" max="1" step="0.1" placeholder="" label="Ignorance" v-model="originalBelief.ignorance" />
            </div>
            <template #footer>
                <div class="footer-dialog">
                    <vs-tooltip>
                        <vs-button flat>
                            ?
                        </vs-button>
                        <template #tooltip>
                            {{ warning }}
                        </template>
                    </vs-tooltip>
                    <vs-button block @click="saveBelief()" color="success" typeof="filled">Save</vs-button>
                    <vs-button block @click="deleteDialogActiv=!deleteDialogActiv" v-show="!(originalBelief.id==-1)" color="danger" typeof="filled">Delete</vs-button>
                </div>
            </template>
        </vs-dialog>
        <!-- Dialogue de confirmation pour la suppression d'une croyance -->
        <vs-dialog v-model="deleteDialogActiv">
            <template #header>
                <h4 class="not-margin">
                    Delete a belief
                </h4>
            </template>
            <div class="con-content">
                <p>
                    Do you want to delete this belief ?
                </p>
            </div>
            <template #footer>
                <div class="footer-dialog">
                    <vs-button block @click="deleteBelief()" color="primary" typeof="filled">Confirm</vs-button>
                </div>
            </template>
        </vs-dialog>
        <vs-dialog v-model="incorrectValueDialog">
            <template #header>
                <h4>Warning</h4>
            </template>
            <div class="con-content">
                <p>
                    Numeric values are incorrect.
                </p>
            </div>
            <template #footer>
                <vs-button block @click="incorrectValueDialog=false" color="primary" typeof="filled">OK</vs-button>
            </template>
        </vs-dialog>
        <!-- Dialogue pour afficher les donn�es (axes non didactiques) -->
        <vs-dialog v-model="valueDialogActiv">
            <template #header>
                <h4>
                    Belief
                </h4>
            </template>
            <div class="con-content" v-show="isBeliefValid()">
                <p>
                    Parent 1 : {{ parent1 != null ? (parent1.name!=null?parent1.name:"Error") : "Error" }}
                </p>
                <p>
                    Parent 2 : {{ parent2 != null ? (parent2.name!=null?parent2.name:"Error") : "Error" }}
                </p>
                <p>
                    Ability : {{ originalBelief.ability!=null?originalBelief.ability:"Error" }}
                </p>
                <p>
                    Disbility : {{ originalBelief.disability!=null?originalBelief.disability:"Error" }}
                </p>
                <p>
                    Conflict : {{ originalBelief.conflict!=null?originalBelief.conflict:"Error" }}
                </p>
                <p>
                    Ignorance : {{ originalBelief.ignorance!=null?originalBelief.ignorance:"Error" }}
                </p>
            </div>
            <div class="con-content" v-show="!isBeliefValid()">
                <p>
                    Parent 1 : {{ parent1 != null ? (parent1.name!=null?parent1.name:"Error") : "Error" }}
                </p>
                <p>
                    Parent 2 : {{ parent2 != null ? (parent2.name!=null?parent2.name:"Error") : "Error" }}
                </p>
                <p>
                    Ignorance : 1
                </p>
            </div>
            <template #footer>
                <vs-button block @click="valueDialogActiv = false" color="primary" typeof="filled">OK</vs-button>
            </template>
        </vs-dialog>
    </td>
</template>

<script>


import 'material-icons/iconfont/material-icons.css'
import axios from 'axios'
axios.defaults.baseURL = '//localhost:5000/'

export default {
    name: 'Belief',
    props : {
        originalBelief: Object,
        parent1: Object,
        parent2: Object,
        didacticMode: Boolean,
        level1: String,
        level2: String, 
    },
    components: {
    },
    data() {
        return {
            addDialogActiv: false,
            deleteDialogActiv: false,
            incorrectValueDialog: false,
            valueDialogActiv: false,
            nameValue: "",
            warning: "The sum of these attributes should equal 1"
        }
    },

        methods: {
        /**
         * Execute les actions et méthodes nécessaires lorsque l'utilisateur sauvegarde le belief
         * */
        saveBelief() {
            if (this.verifValue()) {
                //Cas 1 : edition
                if (this.originalBelief.id!=-1) {
                    this.editBeliefApi()
                    this.addDialogActiv = false
                }
                //Cas 2 : ajout
                else {
                    this.addBeliefApi(this.originalBelief)
                    this.addDialogActiv = false
                }
            }
            else {
                this.incorrectValueDialog = true
            }
            },
            /**
             * Execute les actions et méthodes nécessaires lorsque l'utilisateur veut supprimer le belief (apres confirmation)
             * */
        async deleteBelief() {
            await this.deleteBeliefApi()

            this.originalBelief = {
                        id : -1,
                        color: "rgb(211, 211, 211)",
                        hability: 0,
                        dishability: 0,
                        conflict: 0,
                        ignorance : 0
                        }
            this.addDialogActiv = false
            this.deleteDialogActiv = false
        },
        /**
         * @return vrai si les valeurs numeriques du formulaires sont correctes, faux sinon.
         * */
        verifValue() {
            if (parseFloat(this.originalBelief.ability) + parseFloat(this.originalBelief.disability) + parseFloat(this.originalBelief.conflict) + parseFloat(this.originalBelief.ignorance) != 1) {
                return false
            }
            if (parseFloat(this.originalBelief.ability) > 1 || parseFloat(this.originalBelief.ability) < 0) {
                return false
            }
            if (parseFloat(this.originalBelief.disability) > 1 || parseFloat(this.originalBelief.disability) < 0) {
                return false
            }
            if (parseFloat(this.originalBelief.conflict) > 1 || parseFloat(this.originalBelief.conflict) < 0) {
                return false
            }
            if (parseFloat(this.originalBelief.ignorance) > 1 || parseFloat(this.originalBelief.ignorance) < 0) {
                return false
            }
            return true
            },
        /**
         * Affiche le bon dialogue lorsque l'utilisateur clique sur un Belief
         * */
        openDialog() {
            if (this.didacticMode) {
                this.addDialogActiv = true
            }
            else {
                this.valueDialogActiv = true
            }
        },
        /**
         * Vérifie les confitions numériques du belief dans le formulaire
         * */
        isBeliefValid() {

            return this.originalBelief.ability != null && this.originalBelief.disability != null && this.originalBelief.conflict != null && this.originalBelief.ignorance != null
            },
        /**
         * Ajoute le belief dans la base de donnees
         * */
        addBeliefApi: async function (beliefToAdd) {
            try {
                const belief = {
                    hability: parseFloat(beliefToAdd.ability), 
                    dishability: parseFloat(beliefToAdd.disability),
                    ignorance: parseFloat(beliefToAdd.ignorance),
                    conflict: parseFloat(beliefToAdd.conflict), 
                    interactions: [
                        {
                            nodeId: this.parent1.id,
                            level: this.level1
                        }
                        , {
                            nodeId: this.parent2.id,
                            level: this.level2
                        }
                    ]
                }
                const response = await axios.post("api/people/" + this.$store.state.student + "/believes", belief)
                var belief_formated = {
                    id: response.data.id,
                    ability: response.data.hability, 
                    disability: response.data.dishability,
                    conflict: response.data.conflict, 
                    ignorance: response.data.ignorance,
                    parent1: response.data.interactions[0].nodeId,
                    parent2: response.data.interactions[1].nodeId,
                    value_parent1: response.data.interactions[0].level,
                    value_parent2: response.data.interactions[1].level,
                    student: response.data.personId,
                    color: response.data.colorBelief
                }
                let colors = belief_formated.color.split(",")
                belief_formated.color = "rgba(" + colors[1].substring(3) + "," + colors[2].substring(3) + "," + colors[3].substring(3, colors[3].length - 1) + "," + colors[0].substring(9) + ")"
                //this.originalBelief = belief_formated
                this.$emit('pass-belief', belief_formated)
            }
            catch (error) {
                console.error(error);
            }
        },

        /**
         * Modifie le belief dans la base de donnees
         * */
        editBeliefApi: async function () {
            const deleted_copy= {
                    id:  this.originalBelief.id,
                    ability:  this.originalBelief.ability, 
                    disability:  this.originalBelief.disability,
                    conflict:  this.originalBelief.conflict, 
                    ignorance:  this.originalBelief.ignorance,
                    parent1:  this.originalBelief.parent1,
                    parent2:  this.originalBelief.parent2,
                    value_parent1:  this.originalBelief.value_parent1,
                    value_parent2:  this.originalBelief.value_parent2,
                    student:  this.originalBelief.student,
                    color:  this.originalBelief.color
            }
            await this.deleteBeliefApi()
            console.log("thisoriginalbelief", this.originalBelief)
            this.addBeliefApi(deleted_copy)
        },
        /**
         * Supprime le belief dans la base de donnees
         * */
        async deleteBeliefApi() {
            try{
                await axios.delete("api/people/believes/" + this.originalBelief.id)
                this.$emit('delete-belief', this.originalBelief)
            }catch(error){
                console.log(error)
            }
            }
            
        
    },
    mounted() {
        console.log("Original Belief recu : ", this.originalBelief)
        /*this.abilityValue = this.originalBelief.ability != null ? this.originalBelief.ability : 0
        this.disabilityValue = this.originalBelief.disability != null ? this.originalBelief.disability : 0
        this.conflictValue = this.originalBelief.conflictValue != null ? this.originalBelief.conflict : 0
        this.ignoranceValue = this.originalBelief.ignoranceValue != null ? this.originalBelief.ignorance : 1*/
        if (this.originalBelief.id == -1) {
            this.editMode = false;
            console.log(this.editMode)
        }
    }
}
</script>

<style>
    .vs-input {
        float: left;
        width: 50%;
        margin: 10px;
        margin-top: 5px;
    }
    .con-select {
        margin-left: 10px;
        width: 50%;
        margin-bottom: 10px;
    }
    .vs-button{
        margin: 2%;
    }

    .con-footer {
        display: flex;
        align-items: center;
        justify-content: flex-end;
    }
    .con-footer .vs-button {
        margin: 0px;
    }
    .con-footer .vs-button .vs-button__content{
        padding: 10px 30px;
    }
    .con-footer .vs-button ~.vs-button {
        margin-left: 10px;
    }
    .not-margin {
        margin: 0px;
        font-weight: normal;
        padding: 10px;
        padding-bottom: 0px;
    }
    .con-content {
        width: 100%;
    }
    .con-content p{
        font-size: .8rem;
        padding: 0px 10px;
    }
    .con-form {
        width: 100%;
    }
    .con-form .flex {
        display: flex;
        align-items: center;
        justify-content: space-between;
    }
    .con-form .flex a {
        font-size: .8rem;
        opacity: .7;
    }
    .con-form .flex a :hover {
        opacity: 1;
    }
    .con-form .vs-checkbox-label {
        font-size: .8rem;
    }
    .con-form .vs-input-content {
        margin: 10px 0px;
        width: calc(100%)
    }
    .con-form .vs-input {
        width: 100%;
    }
    .footer-dialog {
        display: flex;
        align-items: center;
        justify-content: center;
        flex-direction: column;
        width: calc(100%);
    }
    .footer-dialog .new {
        margin: 0px;
        margin-top: 20px;
        padding: 0px;
        font-size: .7rem;
    }
    .footer-dialog .new a {
        margin-left: 6px;
    }
    .footer-dialog \&:hover{
        text-decoration: underline;
    }

    td.belief {
        border-style: solid;
        border-color: #000;
        max-height: 30px;
        max-width: 30px;
    }

    td.td-xaxis {
        max-height: 30px;
        max-width: 30px;
    }

    td.td-yaxis {
        text-align: left;
    }
</style>