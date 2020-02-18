##SEO optimization: 
===================

#DESING APPROACH: 
===================
- The project is built using .NET CORE 3.1 and React 16.11.0 (Typescript)
- Since requirement says that we can't use any 3rd party lib and Google API search, the method adopted here is via a testing tool Selenium (third party testing tool) 

* Backend approach: An api is exposed with url as follows. 
                 https://localhost:44327/api/searchRating
                 Input Params : 1) searchString (comma separated string to search) 
                                2) url : The url which matches the search result based on ranking. 
                                3) searchOption: The option to search from 1 -> Google.com 2 -> Bing.com
                                
Using the selenium chrome driver an instance is launched and text is searched. Screen scraping help to capture all the links and form a list of unique links (URL) based on the search.
The process continues until part/full url is matched or we have reached top 100 unique results. 
Then the ranking is calculated and final response in serialized in JSON response to send to the front-end.

                
* Front-end approach: 
The UI is kept simple and intuitive. 
The user needs to enter searchKeyword (separated by Comma) and the url to find the match for ranking. 
The request is submitted in async form and can accept multiple request in one go. 
   
There are notification toasters available to notify the user of the current status or errors if any. (Any backend or network error is taken care of)
The results are displayed in a list format below the page and user has the option to clear the results if required. 
                
# Current status of Project: 
* End to end functionality is working fine.
 

#ENHANCEMENT 
==================
1) Can use Dependency inject to inject Factory class. Makes it easier to test via DI.
2) We can use Redis distributed caching mechanism instead of in memory cache.
3) The project can be scalable by creating docker instances and deploying to any cloud. This would help for auto scaling, high availability and automated deployment. I have deployed in AWS but don't have free limits to host it futher. 