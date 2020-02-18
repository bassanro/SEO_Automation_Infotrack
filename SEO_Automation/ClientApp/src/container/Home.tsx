import axios from "axios";
import * as React from "react";
import { FormEvent } from "react";
import { toast } from "react-toastify";
import withErrorHandler from "../hoc/withErrorHandler";

interface IResult {
  SearchString: string;
  Url: string;
  Ranking: [];
}

type SeachOptionInterface = { label: string; value: number };

class Home extends React.PureComponent {
  state = {
    searchString: "",
    urlString: "",
    selectedOption: 1,
    results: [],
    requestFailed: false
  };

  searchProviders: SeachOptionInterface[] = [
    { label: "Google", value: 1 },
    { label: "Bing", value: 2 }
  ];

  inputChangeHandler = (event: FormEvent<HTMLInputElement>) => {
    this.setState({ searchString: event.currentTarget.value });
  };

  inputURLChangeHandler = (event: FormEvent<HTMLInputElement>) => {
    this.setState({ urlString: event.currentTarget.value });
  };

  cleanSearchResult = () => {
    this.setState({ results: [] });
  };

  handleChange = (event: any) => {
    this.setState({ selectedOption: event.currentTarget.value });
  };

  isEmpty = (str: string | null) => {
    return !str || 0 === str.length;
  };

  getRanking = () => {
    try {
      if (
        this.isEmpty(this.state.searchString.trim()) ||
        this.isEmpty(this.state.urlString.trim())
      ) {
        toast.error("Input fields can't be empty");
        return;
      }

      toast.info("Request sent to backend");
      axios
        .get("https://localhost:44327/api/searchRating", {
          params: {
            searchString: this.state.searchString,
            url: this.state.urlString,
            searchOption: this.state.selectedOption
          }
        })
        .then(response => {
          if (response) {
            this.setState({
              results: [...this.state.results, response.data]
            });
            console.log(response);
          }
        });
    } catch (error) {
      toast.error("Unexpected Error - Make sure Backend server is running");
      this.setState({ requestFailed: true });
      console.error(error);
    }
  };

  render() {
    return (
      <React.Fragment>
        <form>
          <div className="form-group">
            <label>SearchString</label>
            <input
              type="text"
              name="searchString"
              className="form-control"
              value={this.state.searchString}
              onChange={event => this.inputChangeHandler(event)}
            />
          </div>
          <div className="form-group">
            <label>URL</label>
            <input
              type="text"
              name="urlString"
              className="form-control"
              value={this.state.urlString}
              onChange={event => this.inputURLChangeHandler(event)}
            />
          </div>
          <div className="form-group">
            <label>Search Provider</label>
            <select className="form-control" onChange={this.handleChange}>
              {this.searchProviders.map(option => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>
          <button
            type="button"
            className="btn btn-primary mr-3"
            onClick={() => {
              this.getRanking();
            }}
          >
            GET RANKING
          </button>

          <button
            type="button"
            className="btn btn-success mr-3"
            onClick={() => {
              this.cleanSearchResult();
            }}
          >
            CLEAN SEARCH RESULT
          </button>
        </form>

        <div style={{ padding: "50px" }} />
        <ul className="list-group">
          {this.state.results.length
            ? this.state.results.map((result: IResult, index) => {
                return (
                  <li key={index} className="list-group-item">
                    The Ranking of {'"' + result.SearchString + '"'} for URL{" "}
                    {'"' + result.Url + '"'} is {"[" + result.Ranking + "]"}
                  </li>
                );
              })
            : null}
        </ul>
      </React.Fragment>
    );
  }
}

export default withErrorHandler(Home, axios);
