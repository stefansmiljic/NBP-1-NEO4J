import './Admin.css'
import { useState, useEffect, createElement } from "react";
import $ from 'jquery';
import Select from 'react-select';

function Admin() {
    const [gameName, setGameName] = useState("");
    const [thumbURL, setThumbURL] = useState("");
    const [rating, setRating] = useState("");
    const [publisher, setPublisher] = useState("");
    const [genres, setGenres] = useState([]);
    const [igriceData, setIgriceData] = useState([]);
    const [gameId, setGameId] = useState("");
    const [selectedGame, setSelectedGame] = useState(null);

    const handleGameNameChange = (value) => {
        setGameName(value);
      };

      const handleThumbUrlChange = (value) => {
        setThumbURL(value);
      };
      const handleRatingChange = (value) => {
        if(value>5)
            setRating(5);
        else if(value<0)
            setRating(0);
        else
            setRating(value);
      };
      const handlePublisherChange = (value) => {
        setPublisher(value);
      };
      const handleGenresChange = (e) => {
        let value = Array.from(e.target.selectedOptions, option => option.value);
        setGenres(value);
      }

      const handleCreate = () => {
        var model = {
            name: gameName,
            thumbnailURL: thumbURL,
            rating: rating,
            genres: genres,
            publisher: { name: publisher }
        };

        $.ajax({
            type: "POST",
            data: JSON.stringify(model),
            url: "http://localhost:5163/api/Game/CreateGame",
            contentType: "application/json"
        }).done(function(res){
            console.log("res ", res);
        });
      };

      const handleChange = (selectedOption) => {
        setSelectedGame(selectedOption);
        setGameId(selectedOption.value);
      };

      async function getAllGames() {
        try {
            const data = await fetch("http://localhost:5163/api/Game/GetAllGames", {
                method: "GET",
                mode: 'cors',
            });
    
            if (!data.ok) {
                throw new Error(`HTTP error! Status: ${data.status}`);
            }
    
            const returnData = await data.json();
            return returnData;
        } catch (error) {
            console.error("Greška prilikom dohvatanja podataka:", error);
            throw error;
        }
    }

    useEffect(() => {
        getAllGames().then((data) => {
          setIgriceData(data);
        });
      }, []);

      const handleDeleteGame = async () => {
        await fetch(
            "http://localhost:5163/api/Game/DeleteGame?gameId=" + gameId,
            { method: "DELETE" }
        );
        window.location.reload();
    };

    let igrice = [
        ...igriceData.map((igra) => ({ value: igra.id, label: igra.properties.name })),
      ];

    return <div className="mainDiv">
    <h3>Create game</h3>
        <div className="createGame">
            <div className='labelsInputs'>
                <div className="labels">
                    <label>Game name:</label>
                    <label>Thumbnail URL:</label>
                    <label>Rating:</label>
                </div>
                <div className="inputs">
                    <input type="text" className="gameName" onChange={(e) => handleGameNameChange(e.target.value)}></input>
                    <input type="text" className="thumbURL" onChange={(e) => handleThumbUrlChange(e.target.value)}></input>
                    <input type="number" min={0} max={5} step={0.1} className="rating" placeholder='Between 0 and 5' onChange={(e) => handleRatingChange(e.target.value)}></input>
                </div>
            </div>
            <h4>Choose genres</h4>
            <select name="genres" className='genres' id="genres" multiple onChange={(e)=>handleGenresChange(e)}>
                <option value="Action">Action</option>
                <option value="Platform">Platform</option>
                <option value="Puzzle">Puzzle</option>
                <option value="Stealth">Stealth</option>
                <option value="Battle_Royale">Battle Royale</option>
                <option value="MMORPG">MMORPG</option>
                <option value="Rhythm">Rhythm</option>
                <option value="Fighting">Fighting</option>
                <option value="Horror">Horror</option>
                <option value="RPG">RPG</option>
                <option value="RTS">RTS</option>
                <option value="Sandbox">Sandbox</option>
                <option value="Adventure">Adventure</option>
                <option value="Racing">Racing</option>
                <option value="Strategy">Strategy</option>
                <option value="Simulation">Simulation</option>
                <option value="Survival">Survival</option>
                <option value="Shooter">Shooter</option>
                <option value="Sport">Sport</option>
                <option value="Open_World">Open World</option>
                <option value="Side_Scrolling">Side Scrolling</option>
            </select>
            <p className='manual'>In order to select multiple genres, use <b>CTRL + click</b>.</p>
            <div className='labelsInputs'>
            <div className="labels">
                    <label>Publisher:</label>
                </div>
                <div className="inputs">
                    <input type="text" className="publisherName" onChange={(e) => handlePublisherChange(e.target.value)}></input>
                </div>
            </div>
            <input type='button' className='createGameBtn' value={"Create"} onClick={handleCreate}></input>
        </div>
        <h3>Delete game</h3>
        <div className='deleteGame'>
            <Select className="selectGame"
            value={selectedGame}
            onChange={handleChange}
            options={igrice}
            isSearchable
            placeholder="Search for a game..."
            />
            <input type='button' className='deleteGameBtn' onClick={handleDeleteGame} value={"Delete"}></input>
        </div>
    </div>
}

export default Admin;