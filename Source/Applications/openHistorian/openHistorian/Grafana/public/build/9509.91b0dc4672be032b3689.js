"use strict";(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[9509],{99509(_,m,r){r.d(m,{GenAISQLExplainButton:()=>a});var o=r(74848),t=r(96540),i=r(92745),h=r(16200),l=r(37680),u=r(66769),g=r(71260);const e=n=>!n||n.trim()===""?"There is no SQL query to explain. Please enter a SQL expression first.":`${n}

Explain what this query does in simple terms.`,s=(n,c,p,f)=>{const d=(0,g.D)({refIds:n.length>0?n.join(", "):"A",currentQuery:c.trim()||"No current query provided",schemas:p,queryContext:f}),I=e(c);return[{role:u.Xh.system,content:d},{role:u.Xh.user,content:I}]},a=({currentQuery:n,onExplain:c,queryContext:p,refIds:f,schemas:d})=>{const I=(0,t.useCallback)(()=>s(f,n,d,p),[f,n,d,p]),x=n&&n.trim()!=="";return(0,o.jsx)(h.n,{disabled:!x,eventTrackingSrc:l.ec.sqlExpressions,messages:I,onGenerate:c,temperature:.3,text:(0,i.t)("sql-expressions.explain-query","Explain query"),timeout:6e4,toggleTipTitle:(0,i.t)("sql-expressions.ai-explain-title","AI-powered SQL expression explanation"),tooltip:x?(0,i.t)("expressions.sql-expr.tooltip-experimental","SQL Expressions LLM integration is experimental. Please report any issues to the Grafana team."):(0,i.t)("sql-expressions.explain-empty-query-tooltip","Enter a SQL expression to get an explanation")})}},71260(_,m,r){r.d(m,{D:()=>g,o:()=>u});const o={engineInfo:"MySQL dialectic based on dolthub go-mysql-server. The tables are all in memory",refIdExplanation:"RefIDs (A, B, C, etc.) represent data from other queries",columnInfo:"value should always be represented as __value__"},t={refIds:"{refIds}",currentQuery:"{currentQuery}",queryInstruction:"{queryInstruction}",schemaInfo:"{schemaInfo}",errorContext:"{errorContext}",queryContext:"{queryContext}"},i=`You are a SQL expert for Grafana expressions specializing in time series data analysis.
IMPORTANT - Current SQL Errors (if any): ${t.errorContext}

SQL dialect required by Grafana expressions: ${o.engineInfo}

RefIDs context: ${o.refIdExplanation}
Grafana specific context: ${o.columnInfo}

Available RefIDs to use in composable queries: ${t.refIds}

Current query to be improved: ${t.currentQuery}

Schema information to use in composable queries: ${t.schemaInfo}

${t.queryContext}

Query instruction: ${t.queryInstruction}

You may be able to derive schema information from the series data in queryContext.

Given the above data, help users with their SQL query by:
- **PRIORITY: If there are errors listed above, focus on fixing them first**
- Fixing syntax errors using available field and data type information
- Suggesting optimal queries based on actual data schema and patterns.
- Look at query context stats: totalRows, requestTime, numberOfQueries, and if it looks like performance should be part of the conversation, suggest optimizing for performance. Note indexing is not supported in Grafana expressions.
- Leveraging time series patterns and Grafana-specific use cases

Guidelines:
- Use proper field names and types based on schema information
- Include LIMIT clauses for performance unless aggregating
- Consider time-based filtering and grouping for time series data
- Suggest meaningful aggregations for metric data
- Use appropriate JOIN conditions when correlating multiple RefIDs
`,h=`You are an expert in SQL and Grafana SQL expressions with deep knowledge of time series data.

SQL dialect: ${o.engineInfo}

RefIDs: ${o.refIdExplanation}

Grafana specific context: ${o.columnInfo}

Available RefIDs: ${t.refIds}

Schema: ${t.schemaInfo}

${t.queryContext}

Explain SQL queries clearly and concisely, focusing on:
- What data is being selected and from which RefIDs
- How the data is being transformed or aggregated
- The purpose and business meaning of the query using dashboard and panel name from query context if relevant
- Performance implications and optimization opportunities. Database columns can not be indexed in context of Grafana sql expressions. Don't focus on 
  performance unless the query context has a requestTime or totalRows that looks like it could benefit from it.
- Time series specific patterns and their significance

Provide a clear explanation of what this SQL query does:`,l=e=>{if(!e)return"";const s=[];if(e.panelId&&s.push(`Panel Type: ${e.panelId}. Please use this to generate suggestions that are relevant to the panel type.`),e.alerting&&s.push("Context: Alerting rule (focus on boolean/threshold results). Please use this to generate suggestions that are relevant to the alerting rule."),e.queries){const a=Array.isArray(e.queries)?JSON.stringify(e.queries,null,2):String(e.queries);s.push(`Queries available to use in the SQL Expression: ${a}`)}if(e.dashboardContext){const a=typeof e.dashboardContext=="object"?JSON.stringify(e.dashboardContext,null,2):String(e.dashboardContext);s.push(`Dashboard context (dashboard title and panel name): ${a}`)}if(e.datasources){const a=Array.isArray(e.datasources)?JSON.stringify(e.datasources,null,2):String(e.datasources);s.push(`Datasources available to use in the SQL Expression: ${a}`)}if(e.totalRows&&s.push(`Total rows in the query: ${e.totalRows}`),e.requestTime&&s.push(`Request time: ${e.requestTime}`),e.numberOfQueries&&s.push(`Number of queries: ${e.numberOfQueries}`),e.seriesData){const a=typeof e.seriesData=="object"?JSON.stringify(e.seriesData,null,2):String(e.seriesData);s.push(`Series data: ${a}`)}return s.length?`Query Context:
${s.join(`
`)}`:""},u=e=>{const s=l(e.queryContext),a="",n=e.errorContext?.length?e.errorContext.join(`
`):"No current errors detected.";return i.replaceAll(t.refIds,e.refIds).replaceAll(t.currentQuery,e.currentQuery).replaceAll(t.queryInstruction,e.queryInstruction).replaceAll(t.schemaInfo,a).replaceAll(t.errorContext,n).replaceAll(t.queryContext,s)},g=e=>{const s=l(e.queryContext);return h.replaceAll(t.refIds,e.refIds).replaceAll(t.schemaInfo,"").replaceAll(t.queryContext,s)}}}]);

//# sourceMappingURL=9509.91b0dc4672be032b3689.js.map