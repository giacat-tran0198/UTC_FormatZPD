<template>
    <div id="graph">
    <table>
        <tr v-for="(row, rowindex) in belief" v-bind:key="rowindex">
            <td class="td-yaxis"><VueCustomTooltip v-bind:label="yaxis[rowindex].name">
                {{ yaxis[rowindex].name.substring(0,15)+(yaxis[rowindex].name.substring(0,6).length==yaxis[rowindex].name.length?"":"...") }}
                </VueCustomTooltip></td>
                <Belief v-for="(col, colindex) in row" :key="rowindex-colindex" v-bind:didacticMode="didacticAxes" v-bind:originalBelief="col" v-bind:level1="xaxis[colindex].level" v-bind:level2="yaxis[rowindex].level" v-bind:parent1="getParent1(xaxis[colindex])" v-bind:parent2="getParent2(yaxis[rowindex])" v-on:pass-belief="add_belief" v-on:delete-belief="delete_belief">
                </Belief>
        </tr>
        <tr>
            <td class="empty"></td>
            <td v-for="row in xaxis" :key="row" class="td-xaxis">
                <VueCustomTooltip v-bind:label="row.name">
                {{ row.name.substring(0,6)+(row.name.substring(0,6).length==row.name.length?"":"...") }}
                </VueCustomTooltip>
            </td>
        </tr>
    </table>
    </div>
</template>

<script>
import VueCustomTooltip from '@adamdehaven/vue-custom-tooltip';
import Belief from './Belief.vue';
export default {
    name: 'graph',
    components: {
        VueCustomTooltip,
        Belief
    },
    props: {
        belief: Array,
        xaxis: Array,
        yaxis: Array,
        xnode: {},
        ynode: {},
        didacticAxes: Boolean
    },
    data() {
        return {
            
        }
    },
    methods: {
        getParent1(xparent) {
            if (this.didacticAxes) {
                return this.xnode
            }
            else {
                return xparent
            }
        },
        getParent2(yparent) {
            if (this.didacticAxes) {
                return this.ynode
            }
            else {
                return yparent
            }
        },
        add_belief: function(belief){
            this.$emit('add-belief', belief)
        }, 
        delete_belief: function(belief){
            console.log("le belief qu'on veut supprimer dans graph", belief)
            this.$emit('delete-belief', belief)
        }
    }
}
</script>

<style>
    body {
        font-family: Arial, Helvetica, sans-serif;
    }
    td {
        border-width: 2px;
        text-align: center;
        border-spacing: 3em;
        height: 30px;
        width: 30px;
    }
    table {
        border-collapse: separate;
        border-spacing: 3em;
    }
</style>