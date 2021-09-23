<template>
    <div id="home">
        <div>
            <div class="main-zpd">
                <div>
                    <div>

                        <button class="button" v-on:click="show_knowledges()"> Knowledges </button>
                        <button class="button" v-on:click="show_believes()"> Believes </button>
                        <button class="button" v-on:click="clear()">Clear</button>
                    </div>
                    <div class="scroller">
                        <TreeBelieves v-bind:series="beliefGraph()" v-if="belief.length && showBelieves" />
                        <TreeKnowledges v-if="dataReady && showKnowledges" v-bind:series="knowledges" v-bind:show="show" v-on:pass-children="get_axes_values" />
                    </div>
                </div>
                <div class="zpd-legend">

                    <div>
                        <Legend></Legend>
                    </div>
                    <Graph v-bind:belief="beliefTab()" v-bind:xaxis="xrows" v-bind:yaxis="yrows" v-bind:xnode="xnode" v-bind:ynode="ynode" v-bind:didacticAxes="didacticAxes" v-on:add-belief="add_belief" v-on:delete-belief="delete_belief"></Graph>
                </div>
            </div>
        </div>
        <vs-dialog v-model="noStudentDialog">
            <template #header>
                <h4>
                    Warning
                </h4>
            </template>
            <div class="con-content">
                <p>
                    No student has been selected.
                </p>
            </div>
            <template #footer>
                <vs-button block @click="noStudentDialog = false" color="primary" typeof="filled">OK</vs-button>
            </template>
        </vs-dialog>
    </div>
</template>

<script>

    import TreeBelieves from '../components/Trees/TreeBelieves.vue';
    import TreeKnowledges from '../components/Trees/TreeKnowledges.vue';
    import Graph from '../components/zpd/Graph.vue'
    import Legend from '../components/zpd/Legend.vue'
    import 'material-icons/iconfont/material-icons.css'
    import axios from 'axios'
    axios.defaults.baseURL = '//localhost:5000/';
    export default {
        name: 'app',
        components: {
            TreeBelieves,
            TreeKnowledges,
            Graph,
            Legend
        },
        data() {
            return {
                didacticAxes: false,
                noStudentDialog: false,
                showBelieves: false,
                showKnowledges: true,
                knowledges: [],
                belief: [],
                masse: [],
                xrows: [],
                xnode: null,
                yrows: [],
                ynode: null,
                choix_axe: true,
                dataReady: false,
                show: false,

            }
        },
        async mounted() {
            if (this.$store.state.student == null || this.$store.state.student.length <= 0) {
                this.noStudentDialog = true
            }
            try {
                const knowledges = await axios.get("api/nodes")
                for (let idx = 0; idx < knowledges.data.length; idx++) {
                    var parentID = ''
                    if (!knowledges.data[idx].removed) {
                        if (knowledges.data[idx].parentId != '-1') {
                            parentID = knowledges.data[idx].parentId
                        }
                        var knowledge_formated = {
                            id: knowledges.data[idx].id,
                            name: knowledges.data[idx].title,
                            parent: parentID,
                            sibling: '',
                            cls: 'bwhite',
                            "data-type": ''
                        }
                        this.knowledges.push(knowledge_formated)
                        this.dataReady = true;
                    }
                }
            } catch (error) {
                console.log(error)
            }
            this.getBeliefApi()
            this.getBeliefMasseApi()
        },
        methods: {
            show_knowledges: function () {
                this.showBelieves = false
                this.showKnowledges = true
            },
            show_believes: function () {
                this.showBelieves = true
                this.showKnowledges = false
            },
            /**
             * Envoie une requete a la base de donnees pour recuperer la liste de tous les beliefs
             * et les stockes dans la liste this.belief
             * */
            getBeliefApi: async function () {
                try {
                    const believes = await axios.get("api/people/" + this.$store.state.student + "/believes")
                    for (let idx = 0; idx < believes.data.length; idx++) {
                        var belief_formated = {
                            id: believes.data[idx].id,
                            student: believes.data[idx].personId,
                            parent1: believes.data[idx].interactions[0].nodeId,
                            parent2: believes.data[idx].interactions[1].nodeId,
                            value_parent1: believes.data[idx].interactions[0].level,
                            value_parent2: believes.data[idx].interactions[1].level,
                            ability: believes.data[idx].hability,
                            disability: believes.data[idx].dishability,
                            conflict: believes.data[idx].conflict,
                            ignorance: believes.data[idx].ignorance,
                            color: believes.data[idx].colorBelief
                        }
                        let colors = belief_formated.color.split(",")
                        belief_formated.color = "rgba(" + colors[1].substring(3) + "," + colors[2].substring(3) + "," + colors[3].substring(3, colors[3].length - 1) + "," + colors[0].substring(9) + ")"
                        this.belief.push(belief_formated)
                    }
                } catch (error) {
                    console.log(error)
                }
            },
            /**
             * Envoie une requete a la base de donnees pour recuperer la liste de toutes les masses de croyance
             * et les stockes dans la liste this.masse
             * */
            getBeliefMasseApi: async function () {
                try {
                    this.masse = []
                    for (let i = 0; i < this.knowledges.length; i++) {
                        for (let j = 0; j < i; j++) {
                                var parentId1 = this.knowledges[i].id
                                var parentId2 = this.knowledges[j].id
                                const belief = await axios.get("api/people/" + this.$store.state.student + "/believes/parent/" + parentId1 + "/" + parentId2)
                                var belief_formated = {
                                    id: belief.data.id,
                                    ability: belief.data.hability,
                                    disability: belief.data.dishability,
                                    conflict: belief.data.conflict,
                                    ignorance: belief.data.ignorance,
                                    student: belief.data.personId,
                                    color: belief.data.colorBelief,
                                    parent1: parentId1,
                                    parent2: parentId2
                                }
                                let colors = belief_formated.color.split(",")
                                belief_formated.color = "rgba(" + colors[1].substring(3) + "," + colors[2].substring(3) + "," + colors[3].substring(3, colors[3].length - 1) + "," + colors[0].substring(9) + ")"
                                this.masse.push(belief_formated)
                                console.log("les masses sont", this.masse)
                            }
                    }
                } catch (error) {
                    console.log(error)
                }
            },
            /**
             * @return le tableau des belief ordonne tel qu'il sera affiche sur le diagramme zpd
             * */
            beliefTab() {
                //Initialisation du tableau
                var belieftab = []
                for (let index = 0; index < this.yrows.length; index++) {
                    var ytab = []
                    for (let jndex = 0; jndex < this.xrows.length; jndex++) {
                        ytab.push({
                            id: -1,
                            color: "rgb(211, 211, 211)",
                            student: this.$store.state.student,
                            parent1: '',
                            parent2: '',
                            value_parent1: '',
                            value_parent2: '',
                            ability: 0,
                            disability: 0,
                            conflict: 0,
                            ignorance: 1,
                        })
                    }
                    belieftab.push(ytab)
                }
                //Cas 1 : axes didactiques
                //Affichage de beliefs
                if (this.didacticAxes) {
                    for (let index = 0; index < this.yrows.length; index++) {
                        for (let jndex = 0; jndex < this.xrows.length; jndex++) {
                            for (let i = 0; i < this.belief.length; i++) {
                                var newBelief = this.belief[i]
                                if ((this.belief[i].parent1 == this.xrows[jndex].id && this.belief[i].parent2 == this.yrows[index].id)) {
                                    //Pour les didactiques il faut faire attention au level
                                    if (this.xrows[jndex].level == this.belief[i].value_parent1 && this.yrows[index].level == this.belief[i].value_parent2) {
                                        belieftab[index][jndex] = newBelief
                                    }
                                }
                                else if (this.belief[i].parent1 == this.yrows[index].id && this.belief[i].parent2 == this.xrows[jndex].id) {
                                    //Pour les didactiques il faut faire attention au level
                                    if (this.yrows[index].level == this.belief[i].value_parent1 && this.xrows[jndex].level == this.belief[i].value_parent2) {
                                        belieftab[index][jndex] = newBelief
                                    }
                                }
                            }
                        }
                    }
                }
                //Cas 2 : axes non didactiques (ou seulement un axe didactique)
                //Affichage de masse de croyance
                else {
                    for (let index = 0; index < this.yrows.length; index++) {
                        for (let jndex = 0; jndex < this.xrows.length; jndex++) {
                            for (let i = 0; i < this.masse.length; i++) {
                                if ((this.masse[i].parent1 == this.xrows[jndex].id && this.masse[i].parent2 == this.yrows[index].id) || (this.masse[i].parent2 == this.xrows[jndex].id && this.masse[i].parent1 == this.yrows[index].id)) {
                                    if (this.xnode.type == "knowledge" && this.ynode.type == "knowledge") {
                                        var newBelief2 = this.masse[i]
                                        belieftab[index][jndex] = newBelief2
                                        break
                                    }
                                    else {
                                        var newBelief3 = this.masse[i]
                                        belieftab[index][jndex] = newBelief3
                                        break
                                    }
                                }
                            }
                        }
                    }
                }
                return belieftab
            },
            /**
             * 
             * */
            beliefGraph() {
                var beliefgraph = []
                for (let index = 0; index < this.belief.length; index++) {
                    var parent1_name = this.get_parent_name(this.belief[index].parent1)
                    var parent2_name = this.get_parent_name(this.belief[index].parent2)
                    beliefgraph.push({
                        type: 'node',
                        id: this.belief[index].id,
                        text: "Belief" + index.toString(),
                        dataFullName: '',
                        style: {
                            "background-fit": "xy",
                            backgroundColor: this.belief[index].color
                        }
                    })
                    beliefgraph.push({
                        type: 'node',
                        id: this.belief[index].parent1,
                        text: parent1_name + "_" + this.belief[index].value_parent1,
                        dataFullName: '',
                        style: {
                            "background-fit": "xy",
                            backgroundColor: 'rgb(185, 182, 182)'
                        }
                    })
                    beliefgraph.push({
                        type: 'node',
                        id: this.belief[index].parent2,
                        text: parent2_name + "_" + this.belief[index].value_parent2,
                        dataFullName: '',
                        style: {
                            "background-fit": "xy",
                            backgroundColor: 'rgb(185, 182, 182)'
                        }
                    })
                    beliefgraph.push({
                        type: 'link',
                        source: this.belief[index].id,
                        target: this.belief[index].parent1
                    })
                    beliefgraph.push({
                        type: 'link',
                        source: this.belief[index].id,
                        target: this.belief[index].parent2
                    })
                }
                return beliefgraph
            },
            /**
             * Recupere les knowledges selectionnes dans l'arbre des connaissances
             * @param node
             */
            get_axes_values: function (node) {
                //Cas 1 : axe horizontal
                if (this.choix_axe == true) {
                    while (this.xrows.length > 0) {
                        this.xrows.pop();
                    }
                    for (let idx = 0; idx < node.children.length; idx++) {
                        this.xrows.push(node.children[idx])
                    }
                    axios.get("api/nodes/" + node.node)
                        .then(response => {
                            var parentID = ''
                            if (response.data.parentId != '-1') {
                                parentID = response.data.parentId
                            }
                            var knowledge_formated = {
                                id: response.data.id,
                                name: response.data.title,
                                parent: parentID,
                                type: response.data.type,
                                sibling: '',
                                cls: 'bwhite',
                                "data-type": ''
                            }
                            this.xnode = knowledge_formated
                            if (this.ynode != null) {
                                console.log("axe horizontal", this.xnode.type)
                                console.log("axe horizontal",this.ynode.type)
                                if (this.xnode.type == "didactic" && this.ynode.type == "didactic") {
                                    this.didacticAxes = true
                                }
                                else if (this.xnode.type == "knowledge" && this.ynode.type == "knowledge") {
                                    this.didacticAxes = false
                                }
                                else {
                                    this.didacticAxes = false
                                }
                            }
                            else {
                                this.didacticAxes = false
                            }
                        })
                        .catch(error => console.log(error));
                    this.choix_axe = false
                }
                //Cas 2 : axe vertical
                else {
                    while (this.yrows.length > 0) {
                        this.yrows.pop();
                    }
                    for (let idx = 0; idx < node.children.length; idx++) {
                        this.yrows.push(node.children[idx])
                    }

                    axios.get("api/nodes/" + node.node)
                        .then(response => {
                            var parentID = ''
                            if (response.data.parentId != '-1') {
                                parentID = response.data.parentId
                            }
                            var knowledge_formated = {
                                id: response.data.id,
                                name: response.data.title,
                                parent: parentID,
                                type: response.data.type,
                                sibling: '',
                                cls: 'bwhite',
                                "data-type": ''
                            }
                            this.ynode = knowledge_formated
                            
                            console.log("axe vertical    ", this.xnode)
                            console.log("axe vertical    ",this.ynode)

                            if (this.xnode != null) {
                                if (this.xnode.type == "didactic" && this.ynode.type == "didactic") {
                                    this.didacticAxes = true
                                }
                                else if (this.xnode.type == "knowledge" && this.ynode.type == "knowledge") {
                                    this.didacticAxes = false
                                }
                                else {
                                    this.didacticAxes = false
                                }
                            }
                            else {
                                this.didacticAxes = false
                            }
                        })
                        .catch(error => console.log(error));
                    this.choix_axe = true
                }
            },
            /**
             * Renitialise le digramme zpd
             * */
            clear: function () {
                while (this.xrows.length > 0) {
                    this.xrows.pop();
                }
                while (this.yrows.length > 0) {
                    this.yrows.pop();
                }
                this.xnode = null
                this.ynode = null
                this.didacticAxes = false
                this.addBeliefButtonActiv = false
                this.choix_axe = true
            },
            /**
             *
             * @param parentID
             * @return les le nom du pere d'une connaissance
             */
            get_parent_name: function (parentID) {
                for (let idx = 0; idx < this.knowledges.length; idx++) {
                    if (this.knowledges[idx].id == parentID) {
                        return this.knowledges[idx].name
                    }
                }
            },
            add_belief: function (belief) {
                this.belief.push(belief)
                this.getBeliefMasseApi()
                this.beliefTab()
                this.beliefGraph()

            },
            delete_belief: function (beliefToDelete) {
                console.log("Le belief qu'on veut supprimer", beliefToDelete)
                for (let idx = 0; idx < this.belief.length; idx++) {
                    if (this.belief[idx].id == beliefToDelete.id) {
                        this.belief.splice(idx, 1);
                        console.log("belief suprrim�", beliefToDelete.id)
                    }
                }
                this.getBeliefMasseApi()
                this.beliefTab()
                this.beliefGraph()
            }
        }
    }
</script>
<style scoped>
    html {
        height: 100%;
        overflow: hidden;
        scroll-behavior: smooth;
    }

    body {
        border: 0;
        margin: 0;
        padding: 0;
        font-family: 'Lato';
        height: 100%;
        background: rgb(255, 253, 253);
    }

    .main-nav {
        display: flex;
        justify-content: space-between;
        padding: 0.5rem 0.8rem;
    }

    ul.sidebar-panel-nav {
        list-style-type: none;
    }

        ul.sidebar-panel-nav > li > a {
            color: #fff;
            text-decoration: none;
            font-size: 1.5rem;
            display: block;
            padding-bottom: 0.5em;
        }

    .container {
        margin: 30px auto;
        overflow: auto;
        min-height: 80px;
        border: 1px #ccc;
        padding: 30px;
        border-radius: 5px;
        background: #ccc;
    }

    .footer {
        background: #fff;
        position: absolute;
        bottom: 0;
        width: 100%;
        padding-top: 50px;
        height: 100px;
    }

    .button {
        display: inline-block;
        background: rgb(50,100,200);
        color: #fff;
        border: 1px #0a1557;
        border-style: solid;
        padding-right: 8px;
        padding-left: 8px;
        padding-top: 5px;
        padding-bottom: 5px;
        margin: 2px;
        border-radius: 10px;
        cursor: pointer;
        text-decoration: none;
        font-size: 15px;
        font-family: inherit;
    }

        .button:hover {
            background: #0a1557;
        }

        .button:focus {
            background: #0a1557;
        }

    .scroller {
        width: 600px;
        height: 100%;
        overflow-y: scroll;
        overflow-x: scroll;
        scrollbar-color: rebeccapurple green;
        scrollbar-width: thin;
        background-color: white;
    }

.main-zpd{
    display: flex;
    text-align: center;
    padding: 4%;
    width: 100vw;
}



</style>